using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SystemPowerCapablities
    {
        public byte PowerButtonPresent,
            SleepButtonPresent,
            LidPresent,
            SystemS1,
            SystemS2,
            SystemS3,
            SystemS4,
            SystemS5,
            HiberFilePresent,
            FullWake,
            VideoDimPresent,
            ApmPresent,
            UpsPresent,
            ThermalControl,
            ProcessorThrottle,
            ProcessorMinThrottle,
            ProcessorMaxThrottle,
            FastSystemS4;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
        public byte[] spare2;

        public byte DiskSpinDown;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
        public byte[] spare3;

        public byte SystemBatteriesPresent,
            BatteriesAreShortTerm;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
        public BatteryReportingScale[] BatteryScale;

        public SystemPowerState AcOnLineWake,
            SoftLidWake,
            RtcWake,
            MinDeviceWakeState,
            DefaultLowLatencyWake;
    }
}