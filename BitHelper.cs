using System;

namespace ManagedWin32
{
    /// <summary>
    /// Helps perform certain operations on primative types
    /// that deal with bits
    /// </summary>
    public static class BitHelper
    {
        #region Size Constants
        /// <summary>
        /// The max number of bits in byte
        /// </summary>
        public const int BIT_SIZE_BYTE = 8;
        /// <summary>
        /// The max number of bits in short 
        /// </summary>
        public const int BIT_SIZE_SHORT = 16;
        /// <summary>
        /// The max number of bits in int
        /// </summary>
        public const int BIT_SIZE_INT = 32;
        /// <summary>
        /// The max number of bits in long
        /// </summary>
        public const int BIT_SIZE_LONG = 64;
        #endregion

        #region Byte Methods
        /// <summary>
        /// Gets the size of the input value in bits
        /// </summary>
        /// <param name="pInput">The input value</param>
        /// <returns></returns>
        public static int SizeOf(this byte pInput)
        {
            int iRetval = 0;
            if (pInput == 0) iRetval = 0;
            else if (pInput == 1) iRetval = 1;
            else if (pInput < 0) iRetval = BIT_SIZE_BYTE;
            else
            {
                int lTemp = 0;
                for (int i = BIT_SIZE_BYTE - 1; i > 1; i--)
                {
                    lTemp = 1 << i - 1;
                    if ((pInput & lTemp) == lTemp)
                    {
                        iRetval = i;
                        break;
                    }
                }
            }
            return iRetval;
        }

        /// <summary>
        /// Gets a number in the specified range of bits
        /// </summary>
        public static byte GetBits(byte pInput, int pStartIndex, int pLength = BIT_SIZE_BYTE, bool pShift = false)
        {
            int lRetval = 0, lSize = 0, lTemp = 0;
            int lPosition = 1;
            if (pInput < 2 && pInput > 0) return pInput; //Should be either a 0 or 1
            lSize = SizeOf(pInput);

            if (pStartIndex < 1 || pStartIndex > BIT_SIZE_SHORT) throw new ArgumentException("Start bit is out of range.", "pStartIndex");
            if (pLength < 0 || pLength + pStartIndex > BIT_SIZE_BYTE + 1) throw new ArgumentException("End bit is out of range.", "pLength");
            for (int i = pStartIndex; (i < pLength + pStartIndex) && (lPosition <= lSize); i++)
            {
                lTemp = 1 << i - 1;
                if ((pInput & lTemp) == lTemp) lRetval |= (1 << (lPosition - 1));
                lPosition++;
            }
            if (pShift && lPosition < lSize) lRetval <<= lSize - lPosition;
            return (byte)lRetval;
        }

        /// <summary>
        /// Sets the bits.
        /// </summary>
        /// <param name="pDest">The dest.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceIndex">Index of the source.</param>
        /// <param name="pDestIndex">Index of the dest.</param>
        /// <param name="pLength">Length to read.</param>
        public static byte SetBits(byte pDest, byte pSource, int pSourceIndex, int pDestIndex = 0, int pLength = BIT_SIZE_BYTE)
        {
            int lSourceSize = 0, lTemp1 = 0;
            if (pSourceIndex < 1 || pSourceIndex > BIT_SIZE_BYTE) throw new ArgumentException("Start bit is out of range.", "pSourceIndex");
            if (pDestIndex < 0 || pDestIndex > BIT_SIZE_BYTE) throw new ArgumentException("End bit is out of range.", "pDestIndex");
            if (pLength < 0 || pLength + pDestIndex > BIT_SIZE_BYTE) throw new ArgumentException("End bit is out of range.", "pLength");
            pSource = GetBits(pSource, pSourceIndex, pLength);
            lSourceSize = SizeOf(pSource);

            int lPosition = 1;
            for (int i = pDestIndex; (i < lSourceSize + pDestIndex); i++)
            {
                lTemp1 = 1 << lPosition - 1;
                if ((pSource & lTemp1) == lTemp1) pDest |= ((byte)(1 << (i - 1)));
                else
                {
                    lTemp1 = 1 << i - 1;
                    if ((pDest & lTemp1) == lTemp1) pDest ^= ((byte)(1 << (i - 1)));
                }
                lPosition++;
            }
            return (byte)pDest;
        }

        /// <summary>
        /// Determines whether [is bit set] [the specified p input].
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <returns>
        /// 	<c>true</c> if [is bit set] [the specified p input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBitSet(this byte pInput, int pPosition) { return GetBits(pInput, pPosition, 1, false) == 1; }

        /// <summary>
        /// Changes the value of the bit at the specified positon
        /// </summary>
        public static byte ChangeBit(this byte pInput, int pPosition)
        {
            if (pPosition > BIT_SIZE_BYTE) throw new ArgumentException("Position out of range", "pPosition");
            return pInput ^= (byte)(1 << (pPosition - 1));
        }

        /// <summary>
        /// Sets the value of a bit
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <param name="pOn">if set to <c>true</c> [p on].</param>
        public static byte SetBit(this byte pInput, int pPosition, bool pOn)
        {
            if (pPosition > BIT_SIZE_BYTE) throw new ArgumentException("Position out of range", "pPosition");
            bool lIsSet = IsBitSet(pInput, pPosition);
            if (pOn && !lIsSet || pOn && lIsSet) pInput ^= (byte)((1 << (pPosition - 1)));
            return pInput;
        }
        #endregion

        #region Short Methods
        /// <summary>
        /// Checks to see if number is less than 0.
        /// </summary>
        public static bool IsNegative(this short pInputValue) { return (pInputValue & 0x8000) == 0x8000; }

        /// <summary>
        /// Changes the value from positive to negative and vis versa
        /// </summary>
        public static short Negate(this short pInputValue) { return (short)(pInputValue ^ 0x8000); }

        /// <summary>
        /// Gets the size of the input value in bits
        /// </summary>
        /// <param name="pInput">The input value</param>
        public static int SizeOf(this short pInput)
        {
            int iRetval = 0;
            if (pInput == 0) iRetval = 0;
            else if (pInput == 1) iRetval = 1;
            else if (pInput < 0) iRetval = BIT_SIZE_SHORT;
            else
            {
                int lTemp = 0;
                for (int i = BIT_SIZE_SHORT - 1; i > 1; i--)
                {
                    lTemp = 1 << i - 1;
                    if ((pInput & lTemp) == lTemp)
                    {
                        iRetval = i;
                        break;
                    }
                }
            }
            return iRetval;
        }

        /// <summary>
        /// Gets a number in the specified range of bits
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns></returns>
        public static short GetBits(this short pInput, int pStartIndex, int pLength = BIT_SIZE_SHORT, bool pShift = false)
        {
            int lRetval = 0, lSize = 0, lTemp = 0;
            int lPosition = 1;
            if (pInput < 2 && pInput > 0)
            {
                return pInput; //Should be either a 0 or 1
            }
            lSize = SizeOf(pInput);


            if (pStartIndex < 1 || pStartIndex > BIT_SIZE_SHORT)
            {
                throw new ArgumentException("Start bit is out of range.", "pStartIndex");
            }
            if (pLength < 0 || pLength + pStartIndex > BIT_SIZE_SHORT + 1)
            {
                throw new ArgumentException("End bit is out of range.", "pLength");
            }
            for (int i = pStartIndex; (i < pLength + pStartIndex) && (lPosition <= lSize); i++)
            {
                lTemp = 1 << i - 1;
                if ((pInput & lTemp) == lTemp)
                {
                    lRetval |= (1 << (lPosition - 1));
                }
                lPosition++;
            }
            if (pShift && lPosition < lSize)
            {
                lRetval <<= lSize - lPosition;
            }
            return (short)lRetval;
        }

        /// <summary>
        /// Sets the bits.
        /// </summary>
        /// <param name="pDest">The dest.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceIndex">Index of the source.</param>
        /// <param name="pDestIndex">Index of the dest.</param>
        /// <param name="pLength">Length to read.</param>
        /// <returns></returns>
        public static short SetBits(short pDest, short pSource, int pSourceIndex,
            int pDestIndex = 0, int pLength = BIT_SIZE_SHORT)
        {
            int lSourceSize = 0, lTemp1 = 0;
            if (pSourceIndex < 1 || pSourceIndex > BIT_SIZE_SHORT)
            {
                throw new ArgumentException("Start bit is out of range.", "pSourceIndex");
            }
            if (pDestIndex < 0 || pDestIndex > BIT_SIZE_SHORT)
            {
                throw new ArgumentException("End bit is out of range.", "pDestIndex");
            }
            if (pLength < 0 || pLength + pDestIndex > BIT_SIZE_SHORT)
            {
                throw new ArgumentException("End bit is out of range.", "pLength");
            }
            pSource = GetBits(pSource, pSourceIndex, pLength);
            lSourceSize = SizeOf(pSource);

            int lPosition = 1;
            for (int i = pDestIndex; (i < lSourceSize + pDestIndex); i++)
            {
                lTemp1 = 1 << lPosition - 1;
                if ((pSource & lTemp1) == lTemp1)
                {
                    pDest |= ((short)(1 << (i - 1)));
                }
                else
                {
                    lTemp1 = 1 << i - 1;
                    if ((pDest & lTemp1) == lTemp1)
                    {
                        pDest ^= ((short)(1 << (i - 1)));
                    }
                }
                lPosition++;
            }
            return pDest;
        }

        /// <summary>
        /// Determines whether [is bit set] [the specified p input].
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <returns><c>true</c> if [is bit set] [the specified p input]; otherwise, <c>false</c>.</returns>
        public static bool IsBitSet(this short pInput, int pPosition) { return GetBits(pInput, pPosition, 1, false) == 1; }

        /// <summary>
        /// Changes the value of the bit at the specified positon
        /// </summary>
        /// <param name="pInput"></param>
        /// <param name="pPosition"></param>
        /// <returns></returns>
        public static short ChangeBit(this short pInput, int pPosition)
        {
            if (pPosition > BIT_SIZE_SHORT) throw new ArgumentException("Position out of range", "pPosition");
            return pInput ^= (short)(1 << (pPosition - 1));
        }

        /// <summary>
        /// Sets the value of a bit
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <param name="pOn">if set to <c>true</c> [p on].</param>
        public static short SetBit(this short pInput, int pPosition, bool pOn)
        {
            if (pPosition > BIT_SIZE_SHORT) throw new ArgumentException("Position out of range", "pPosition");
            bool lIsSet = IsBitSet(pInput, pPosition);
            if (pOn && !lIsSet || pOn && lIsSet) pInput ^= (short)(1 << (pPosition - 1));
            return pInput;
        }

        /// <summary>
        /// The return value is the high-order byte of the specified value.
        /// </summary>
        public static byte HiByte(this short pWord) { return ((byte)(((short)(pWord) >> 8) & 0xFF)); }

        /// <summary>
        /// The return value is the low-order byte of the specified value.
        /// </summary>
        public static byte LoByte(this short pWord) { return ((byte)pWord); }
        #endregion

        #region Int Methods
        /// <summary>
        /// Checks to see if number is less than 0.
        /// </summary>
        /// <param name="pInputValue"></param>
        /// <returns></returns>
        public static bool IsNegative(this int pInputValue) { return (pInputValue & 0x80000000) == 0x80000000; }

        /// <summary>
        /// Changes the value from positive to negative and vis versa
        /// </summary>
        /// <param name="pInputValue">The value</param>
        /// <returns></returns>
        public static int Negate(this int pInputValue) { return (int)(pInputValue ^ 0x80000000); }

        /// <summary>
        /// Gets the size of the input value in bits
        /// </summary>
        /// <param name="pInput">The input value</param>
        /// <returns></returns>
        public static int SizeOf(this int pInput)
        {
            int iRetval = 0;
            if (pInput == 0) iRetval = 0;
            else if (pInput == 1) iRetval = 1;
            else if (pInput < 0) iRetval = BIT_SIZE_INT;
            else
            {
                int lTemp = 0;
                for (int i = BIT_SIZE_INT - 1; i > 1; i--)
                {
                    lTemp = 1 << i - 1;
                    if ((pInput & lTemp) == lTemp)
                    {
                        iRetval = i;
                        break;
                    }
                }
            }
            return iRetval;
        }

        /// <summary>
        /// Gets a number in the specified range of bits
        /// </summary>
        public static int GetBits(this int pInput, int pStartIndex, int pLength = BIT_SIZE_INT, bool pShift = false)
        {
            int lRetval = 0, lSize = 0, lTemp = 0;
            int lPosition = 1;
            if (pInput < 2 && pInput > 0) return pInput; //Should be either a 0 or 1
            lSize = SizeOf(pInput);

            if (pStartIndex < 1 || pStartIndex > BIT_SIZE_INT) throw new ArgumentException("Start bit is out of range.", "pStartIndex");
            if (pLength < 0 || pLength + pStartIndex > BIT_SIZE_INT + 1) throw new ArgumentException("End bit is out of range.", "pLength");
            for (int i = pStartIndex; (i < pLength + pStartIndex) && (lPosition <= lSize); i++)
            {
                lTemp = 1 << i - 1;
                if ((pInput & lTemp) == lTemp) lRetval |= (1 << (lPosition - 1));
                lPosition++;
            }
            if (pShift && lPosition < lSize) lRetval <<= lSize - lPosition;
            return lRetval;
        }

        /// <summary>
        /// Sets the bits.
        /// </summary>
        /// <param name="pDest">The dest.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceIndex">Index of the source.</param>
        /// <param name="pDestIndex">Index of the dest.</param>
        /// <param name="pLength">Length to read.</param>
        /// <returns></returns>
        public static int SetBits(int pDest, int pSource, int pSourceIndex, int pDestIndex = 0, int pLength = BIT_SIZE_INT)
        {
            int lSourceSize = 0, lTemp1 = 0;
            if (pSourceIndex < 1 || pSourceIndex > BIT_SIZE_INT) throw new ArgumentException("Start bit is out of range.", "pSourceIndex");
            if (pDestIndex < 0 || pDestIndex > BIT_SIZE_INT) throw new ArgumentException("End bit is out of range.", "pDestIndex");
            if (pLength < 0 || pLength + pDestIndex > BIT_SIZE_INT) throw new ArgumentException("End bit is out of range.", "pLength");
            pSource = GetBits(pSource, pSourceIndex, pLength);
            lSourceSize = SizeOf(pSource);

            int lPosition = 1;
            for (int i = pDestIndex; (i < lSourceSize + pDestIndex); i++)
            {
                lTemp1 = 1 << lPosition - 1;
                if ((pSource & lTemp1) == lTemp1) pDest |= (1 << (i - 1));
                else
                {
                    lTemp1 = 1 << i - 1;
                    if ((pDest & lTemp1) == lTemp1) pDest ^= (1 << (i - 1));
                }
                lPosition++;
            }
            return pDest;
        }

        /// <summary>
        /// Determines whether [is bit set] [the specified p input].
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <returns>
        /// 	<c>true</c> if [is bit set] [the specified p input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBitSet(this int pInput, int pPosition) { return GetBits(pInput, pPosition, 1, false) == 1; }

        /// <summary>
        /// Changes the value of the bit at the specified positon
        /// </summary>
        /// <param name="pInput"></param>
        /// <param name="pPosition"></param>
        /// <returns></returns>
        public static int ChangeBit(this int pInput, int pPosition)
        {
            if (pPosition > BIT_SIZE_INT) throw new ArgumentException("Position out of range", "pPosition");
            return pInput ^= (1 << (pPosition - 1));
        }

        /// <summary>
        /// Sets the value of a bit
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <param name="pOn">if set to <c>true</c> [p on].</param>
        /// <returns></returns>
        public static int SetBit(this int pInput, int pPosition, bool pOn)
        {
            if (pPosition > BIT_SIZE_INT) throw new ArgumentException("Position out of range", "pPosition");
            bool lIsSet = IsBitSet(pInput, pPosition);
            if (pOn && !lIsSet || pOn && lIsSet) pInput ^= (1 << (pPosition - 1));
            return pInput;
        }

        /// <summary>
        /// The return value is the high-order word of the specified value.
        /// </summary>
        public static short HiWord(this int pDWord) { return ((short)(((pDWord) >> 16) & 0xFFFF)); }

        /// <summary>
        /// The return value is the low-order word of the specified value.
        /// </summary>
        public static short LoWord(this int pDWord) { return ((short)pDWord); }
        #endregion

        #region Long Methods
        /// <summary>
        /// Checks to see if number is less than 0.
        /// </summary>
        public static bool IsNegative(this long pInputValue) { return (((ulong)pInputValue) & 0x8000000000000000) == 0x8000000000000000; }

        /// <summary>
        /// Changes the value from positive to negative and vis versa
        /// </summary>
        public static long Negate(this long pInputValue) { return (long)(((ulong)pInputValue) ^ 0x8000000000000000); }

        /// <summary>
        /// Gets the size of the input value in bits
        /// </summary>
        public static int SizeOf(this long pInput)
        {
            int iRetval = 0;
            if (pInput == 0) iRetval = 0;
            else if (pInput == 1) iRetval = 1;
            else if (pInput < 0) iRetval = BIT_SIZE_LONG;
            else
            {
                long lTemp = 0;
                for (int i = BIT_SIZE_LONG - 1; i > 1; i--)
                {
                    lTemp = 1 << i - 1;
                    if ((pInput & lTemp) == lTemp)
                    {
                        iRetval = i;
                        break;
                    }
                }
            }
            return iRetval;
        }

        /// <summary>
        /// Gets a number in the specified range of bits
        /// </summary>
        public static long GetBits(this long pInput, int pStartIndex, int pLength = BIT_SIZE_LONG, bool pShift = false)
        {
            long lRetval = 0, lSize = 0, lTemp = 0;
            long lPosition = 1;
            if (pInput < 2 && pInput > 0) return pInput; //Should be either a 0 or 1
            lSize = SizeOf(pInput);

            if (pStartIndex < 1 || pStartIndex > BIT_SIZE_LONG) throw new ArgumentException("Start bit is out of range.", "pStartIndex");
            if (pLength < 0 || pLength + pStartIndex > BIT_SIZE_LONG + 1) throw new ArgumentException("End bit is out of range.", "pLength");
            for (int i = pStartIndex; (i < pLength + pStartIndex) && (lPosition <= lSize); i++)
            {
                lTemp = 1 << i - 1;
                if ((pInput & lTemp) == lTemp) lRetval |= (1 << ((int)(lPosition - 1)));
                lPosition++;
            }
            if (pShift && lPosition < lSize) lRetval <<= ((int)(lSize - lPosition));
            return lRetval;
        }

        /// <summary>
        /// Sets the bits.
        /// </summary>
        /// <param name="pDest">The dest.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceIndex">Index of the source.</param>
        /// <param name="pDestIndex">Index of the dest.</param>
        /// <param name="pLength">Length to read.</param>
        public static long SetBits(long pDest, long pSource, int pSourceIndex, int pDestIndex = 0, int pLength = BIT_SIZE_LONG)
        {
            long lSourceSize = 0, lTemp1 = 0;
            if (pSourceIndex < 1 || pSourceIndex > BIT_SIZE_LONG) throw new ArgumentException("Start bit is out of range.", "pSourceIndex");
            if (pDestIndex < 0 || pDestIndex > BIT_SIZE_LONG) throw new ArgumentException("End bit is out of range.", "pDestIndex");
            if (pLength < 0 || pLength + pDestIndex > BIT_SIZE_LONG) throw new ArgumentException("End bit is out of range.", "pLength");
            pSource = GetBits(pSource, pSourceIndex, pLength);
            lSourceSize = SizeOf(pSource);

            int lPosition = 1;
            for (int i = pDestIndex; (i < lSourceSize + pDestIndex); i++)
            {
                lTemp1 = 1 << lPosition - 1;
                if ((pSource & lTemp1) == lTemp1) pDest |= (1 << (i - 1));
                else
                {
                    lTemp1 = 1 << i - 1;
                    if ((pDest & lTemp1) == lTemp1) pDest ^= (1 << (i - 1));
                }
                lPosition++;
            }
            return pDest;
        }


        /// <summary>
        /// Determines whether [is bit set] [the specified p input].
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <returns>
        /// 	<c>true</c> if [is bit set] [the specified p input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBitSet(this long pInput, int pPosition) { return GetBits(pInput, pPosition, 1, false) == 1; }

        /// <summary>
        /// Changes the value of the bit at the specified positon
        /// </summary>
        /// <param name="pInput"></param>
        /// <param name="pPosition"></param>
        /// <returns></returns>
        public static long ChangeBit(this long pInput, int pPosition)
        {
            if (pPosition > BIT_SIZE_LONG) throw new ArgumentException("Position out of range", "pPosition");
            return pInput ^= (1 << (pPosition - 1));
        }

        /// <summary>
        /// Sets the value of a bit
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <param name="pOn">if set to <c>true</c> [p on].</param>
        /// <returns></returns>
        public static long SetBit(this long pInput, int pPosition, bool pOn)
        {
            if (pPosition > BIT_SIZE_LONG) throw new ArgumentException("Position out of range", "pPosition");
            bool lIsSet = IsBitSet(pInput, pPosition);
            if (pOn && !lIsSet || pOn && lIsSet) pInput ^= (1 << (pPosition - 1));
            return pInput;
        }

        /// <summary>
        /// The return value is the high-order double word of the specified value.
        /// </summary>
        public static int HiDword(this long pDWord) { return ((int)(((pDWord) >> 32) & 0xFFFFFFFF)); }

        /// <summary>
        /// The return value is the low-order word of the specified value.
        /// </summary>
        public static int LoDword(this long pDWord) { return ((int)pDWord); }
        #endregion

        #region Make
        /// <summary>
        /// Makes a 64 bit long from two 32 bit integers
        /// </summary>
        /// <param name="pValueLow">The low order value.</param>
        /// <param name="pValueHigh">The high order value.</param>
        public static long MakeLong(int pValueLow, int pValueHigh)
        {
            if (pValueHigh == 0) return (long)pValueLow;

            long lTemp = SizeOf(pValueHigh);
            lTemp = (pValueHigh << ((BIT_SIZE_LONG) - ((int)lTemp + 1)));
            return (long)(pValueLow | lTemp);
        }

        /// <summary>
        /// Makes a 32 bit integer from two 16 bit shorts
        /// </summary>
        /// <param name="pValueLow">The low order value.</param>
        /// <param name="pValueHigh">The high order value.</param>
        public static int MakeDword(short pValueLow, short pValueHigh)
        {
            if (pValueHigh == 0) return (int)pValueLow;

            int lTemp = SizeOf(pValueHigh);
            lTemp = pValueHigh << ((BIT_SIZE_INT) - (lTemp + 1));
            return (int)(lTemp | pValueLow);
        }

        /// <summary>
        /// Makes a 16 bit short from two bytes
        /// </summary>
        /// <param name="pValueLow">The low order value.</param>
        /// <param name="pValueHigh">The high order value.</param>
        public static short MakeWord(byte pValueLow, byte pValueHigh)
        {
            if (pValueHigh == 0) return (short)pValueLow;

            int lTemp = SizeOf(pValueHigh);
            lTemp = pValueHigh << ((BIT_SIZE_SHORT) - (lTemp + 1));
            return (short)(pValueLow | lTemp);
        }
        #endregion
    }
}