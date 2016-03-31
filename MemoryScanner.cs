using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ManagedWin32
{
    public abstract class MemoryScan<T> where T : IEquatable<T>
    {
        protected MemoryScan(Process Process) { reader = new ProcessMemory(Process); }

        #region Constant fields
        //Maximum memory block size to read in every read process.

        //Experience tells me that,
        //if ReadStackSize be bigger than 20480, there will be some problems
        //retrieving correct blocks of memory values.
        protected const int ReadStackSize = 20480;
        #endregion

        //Instance of ProcessMemoryReader class to be used to read the memory.
        protected ProcessMemory reader;

        //New thread object to run the scan in
        protected Thread thread;

        public abstract void Start(T Value);

        public abstract void Cancel();

        #region Events
        public event Action<int> ScanProgressChanged;

        protected void OnScanProgressChanged(int Progress) => ScanProgressChanged?.Invoke(Progress);

        public event Action<int[]> ScanCompleted;

        protected void OnScanCompleted(int[] Value) => ScanCompleted?.Invoke(Value);

        public event Action ScanCanceled;

        protected void OnScanCancelled() => ScanCanceled?.Invoke();
        #endregion
    }

    public class RegularMemoryScan<T> : MemoryScan<T> where T : IEquatable<T>
    {
        //Start and End addresses to be scaned.
        IntPtr baseAddress, lastAddress;

        //Class entry point.
        //The process, StartAddress and EndAdrress will be defined in the class definition.
        public RegularMemoryScan(Process process, int StartAddress, int EndAddress)
            : base(process)
        {
            //Set the Start and End addresses of the scan to what is wanted.
            baseAddress = (IntPtr)StartAddress;
            lastAddress = (IntPtr)EndAddress;//The scan starts from baseAddress,
            //and progresses up to EndAddress.
        }
        
        public override void Start(T Value)
        {
            Cancel();

            thread = new Thread(() => Scanner(Value));

            thread.Start();
        }

        //Cancel the scan started.
        public override void Cancel()
        {
            if (thread == null)
                return;

            //If the thread is already defined and is Alive,
            if (!thread.IsAlive)
                return;

            OnScanCancelled();

            //and then abort the thread that scanes the memory.
            thread.Abort();
        }

        void Scanner(T Value)
        {
            //The difference of scan start point in all loops except first loop,
            //that doesn't have any difference, is type's Bytes count minus 1.
            var BytesCount = typeof(T) == typeof(short) ? 2
                : typeof(T) == typeof(int) ? 4
                : typeof(T) == typeof(long) ? 8
                : 0;

            var arraysDifference = BytesCount - 1;

            //Define a List object to hold the found memory addresses.
            var finalList = new List<int>();

            //Open the pocess to read the memory.
            reader.Open();

            //Calculate the size of memory to scan.
            var memorySize = (int)lastAddress - (int)baseAddress;

            //If more that one block of memory is requered to be read,
            if (memorySize >= ReadStackSize)
            {
                //Count of loops to read the memory blocks.
                var loopsCount = memorySize / ReadStackSize;

                //Look to see if there is any other bytes let after the loops.
                var outOfBounds = memorySize % ReadStackSize;

                //Set the currentAddress to first address.
                var currentAddress = (int)baseAddress;

                //This will be used to check if any bytes have been read from the memory.
                int bytesReadSize;

                //Set the size of the bytes blocks.
                var bytesToRead = ReadStackSize;

                //An array to hold the bytes read from the memory.
                byte[] array;

                for (var i = 0; i < loopsCount; i++)
                {
                    //Calculte and set the progress percentage.
                    OnScanProgressChanged((int)(((currentAddress - (int)baseAddress) / (double)memorySize) * 100d));

                    //Read the bytes from the memory.
                    array = reader.Read((IntPtr)currentAddress, (uint)bytesToRead, out bytesReadSize);

                    //If any byte is read from the memory (there has been any bytes in the memory block),
                    if (bytesReadSize > 0)
                    {
                        //Loop through the bytes one by one to look for the values.
                        for (var j = 0; j < array.Length - arraysDifference; j++)
                        {
                            if (typeof(T) == typeof(short))
                            {
                                if (Value.Equals(BitConverter.ToInt16(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                if (Value.Equals(BitConverter.ToInt32(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                if (Value.Equals(BitConverter.ToInt64(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                        }
                    }
                    //Move currentAddress after the block already scaned, but
                    //move it back some steps backward (as much as arraysDifference)
                    //to avoid loosing any values at the end of the array.
                    currentAddress += array.Length - arraysDifference;

                    //Set the size of the read block, bigger, to  the steps backward.
                    //Set the size of the read block, to fit the back steps.
                    bytesToRead = ReadStackSize + arraysDifference;
                }
                //If there is any more bytes than the loops read,
                if (outOfBounds > 0)
                {
                    //Read the additional bytes.
                    var outOfBoundsBytes = reader.Read((IntPtr)currentAddress, (uint)((int)lastAddress - currentAddress), out bytesReadSize);

                    //If any byte is read from the memory (there has been any bytes in the memory block),
                    if (bytesReadSize > 0)
                    {
                        //Loop through the bytes one by one to look for the values.
                        for (var j = 0; j < outOfBoundsBytes.Length - arraysDifference; j++)
                        {
                            if (typeof(T) == typeof(short))
                            {
                                //If any value is equal to what we are looking for,
                                if (Value.Equals(BitConverter.ToInt16(outOfBoundsBytes, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                //If any value is equal to what we are looking for,
                                if (Value.Equals(BitConverter.ToInt32(outOfBoundsBytes, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                //If any value is equal to what we are looking for,
                                if (Value.Equals(BitConverter.ToInt64(outOfBoundsBytes, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                        }
                    }
                }
            }
            //If the block could be read in just one read,
            else
            {
                //Calculate the memory block's size.
                var blockSize = memorySize % ReadStackSize;

                //Set the currentAddress to first address.
                var currentAddress = (int)baseAddress;

                //Holds the count of bytes read from the memory.

                //If the memory block can contain at least one 16 bit variable.
                if (blockSize > BytesCount)
                {
                    //Read the bytes to the array.
                    int bytesReadSize;
                    var array = reader.Read((IntPtr)currentAddress, (uint)blockSize, out bytesReadSize);

                    //If any byte is read,
                    if (bytesReadSize > 0)
                    {
                        //Loop through the array to find the values.
                        for (var j = 0; j < array.Length - arraysDifference; j++)
                        {
                            if (typeof(T) == typeof(short))
                            {
                                if (Value.Equals(BitConverter.ToInt16(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                if (Value.Equals(BitConverter.ToInt32(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                if (Value.Equals(BitConverter.ToInt64(array, j)))
                                    //add it's memory address to the finalList.
                                    finalList.Add(j + currentAddress);
                            }
                        }
                    }
                }
            }
            //Close the handle to the process to avoid process errors.
            reader.Dispose();

            //Prepare the ScanProgressed and set the progress percentage to 100% and raise the event.
            OnScanProgressChanged(100);

            //Prepare and raise the ScanCompleted event.
            OnScanCompleted(finalList.ToArray());
        }
    }

    public class IrregularMemoryScan<T> : MemoryScan<T> where T : IEquatable<T>
    {
        //An array to hold the addresses.
        int[] addresses;

        //Class entry point.
        //The process, StartAddress and EndAdrress will be defined in the class definition.
        public IrregularMemoryScan(Process process, int[] AddressesList)
            : base(process)
        {
            //Take the addresses list and store them in local array.
            addresses = AddressesList;
        }

        public override void Start(T Value)
        {
            Cancel();

            thread = new Thread(o => Scanner(Value));

            thread.Start();
        }

        //Cancel the scan started.
        public override void Cancel()
        {
            if (thread == null)
                return;

            //If the thread is already defined and is Alive,
            if (!thread.IsAlive)
                return;

            OnScanCancelled();

            //and then abort the alive thread and so cancel last scan task.
            thread.Abort();
        }

        void Scanner(T Value)
        {
            var BytesCount = typeof(T) == typeof(short) ? 2u
                : typeof(T) == typeof(int) ? 4u
                : typeof(T) == typeof(long) ? 8u
                : 0;

            //Define a List object to hold the found memory addresses.
            var finalList = new List<int>();

            //Open the pocess to read the memory.
            reader.Open();

            //This will be used to check if any bytes have been read from the memory.

            //An array to hold the bytes read from the memory.

            for (var i = 0; i < addresses.Length; i++)
            {
                //Calculte and set the progress percentage.
                OnScanProgressChanged((int)((i / (double)addresses.Length) * 100d));

                //Read the bytes from the memory.
                int bytesReadSize;
                var array = reader.Read((IntPtr)addresses[i], BytesCount, out bytesReadSize);

                //If any byte is read from the memory (there has been any bytes in the memory block),
                if (bytesReadSize <= 0)
                    continue;

                if (typeof(T) == typeof(short))
                {
                    if (Value.Equals(BitConverter.ToInt16(array, 0)))
                        //add it's memory address to the finalList.
                        finalList.Add(addresses[i]);
                }
                else if (typeof(T) == typeof(int))
                {
                    if (Value.Equals(BitConverter.ToInt32(array, 0)))
                        //add it's memory address to the finalList.
                        finalList.Add(addresses[i]);
                }
                else if (typeof(T) == typeof(long))
                {
                    if (Value.Equals(BitConverter.ToInt64(array, 0)))
                        //add it's memory address to the finalList.
                        finalList.Add(addresses[i]);
                }
            }
            //Close the handle to the process to avoid process errors.
            reader.Dispose();

            //Prepare the ScanProgressed and set the progress percentage to 100% and raise the event.
            OnScanProgressChanged(100);

            OnScanCompleted(finalList.ToArray());
        }
    }

    public class MemoryFreeze<T> where T : IEquatable<T>
    {
        //Struct with 3 properties to retrieve a memory addresse's information.
        public class MemoryRecords
        {
            public MemoryRecords(int Address, T Value)
            {
                this.Address = Address;
                this.Value = Value;
            }

            //Public properties.
            public int Address { get; internal set; }

            public T Value { get; internal set; }

            public Type Type => typeof(T);
        }
     
        //List of memoryRecords to hold the addresses and their related information.
        List<MemoryRecords> records;

        //A ProcessMemoryReader object to write the values to the memory addresses.
        ProcessMemory writer;

        //Timer object to tick and freeze.
        System.Timers.Timer timer;
     
        //a property to retrieve the current list of memory addresses set to be freezed.
        public MemoryRecords[] FreezedMemoryAddresses => records.ToArray();
     
        public MemoryFreeze(Process process)
        {
            timer = new System.Timers.Timer();

            writer = new ProcessMemory(process);

            records = new List<MemoryRecords>();

            timer.Elapsed += TimerElapsed;
        }

        //Add a memory address and a 16 bit value to be written in the address.
        public void AddMemoryAddress(int MemoryAddress, T Value) => records.Add(new MemoryRecords(MemoryAddress, Value));

        //Start the timer with the given Interval to start looping and so start freezing.
        public void Start(double Interval)
        {
            timer.Interval = Interval;
            timer.Start();
        }

        //Stop the timer and so stop the freezing loops.
        public void Stop() => timer.Stop();
        
        //The method will be called in every timer's ticking.
        void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Open the process.
            writer.Open();

            //Loop and set the value of all addresses.
            foreach (var t in records)
            {
                if (t.Type == typeof(short))
                    writer.Write((IntPtr)t.Address, BitConverter.GetBytes((short)(object)t.Value));

                else if (t.Type == typeof(int))
                    writer.Write((IntPtr)t.Address, BitConverter.GetBytes((int)(object)t.Value));

                else if (t.Type == typeof(long))
                    writer.Write((IntPtr)t.Address, BitConverter.GetBytes((long)(object)t.Value));
            }

            //Close the handle to the process.
            writer.Dispose();
        }
    }
}