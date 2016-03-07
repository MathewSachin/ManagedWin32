using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct BatteryReportingScale
    {
        public int Granularity,
                   Capacity;
    }
}