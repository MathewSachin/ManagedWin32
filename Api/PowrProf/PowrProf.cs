using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    public static class PowerProf
    {
        const string DllName = "powrprof.dll";

        [DllImport(DllName)]
        public static extern int CallNtPowerInformation(int InformationLevel, IntPtr lpInputBuffer,
            int nInputBufferSize, ref SystemPowerCapablities lpOutputBuffer, int nOutputBufferSize);

        [DllImport(DllName)]
        static extern PowerPlatformRole PowerDeterminePlatformRole();
        
        public static PowerPlatformRole PowerPlatformRole => PowerDeterminePlatformRole();

        /// <summary>
        /// Full call to method PowerSettingAccessCheck().
        /// </summary>
        /// <param name="AccessFlags">One or more check specifier flags</param>
        /// <param name="PowerGuid">The relevant Power Policy GUID</param>
        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int PowerSettingAccessCheck(PowerDataAccessor AccessFlags, [MarshalAs(UnmanagedType.LPStruct)] Guid PowerGuid);

        [DllImport(DllName)]
        public static extern int PowerReadACValueIndex(int RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Value);

        [DllImport(DllName)]
        public static extern int PowerReadDCValueIndex(int RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Value);

        [DllImport(DllName)]
        public static extern int PowerReadACDefaultIndex(int RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Value);

        [DllImport(DllName)]
        public static extern int PowerReadDCDefaultIndex(int RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Value);

        [DllImport(DllName)]
        public static extern int PowerWriteACValueIndex(int RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, int AcValueIndex);

        [DllImport(DllName)]
        public static extern int PowerWriteDCValueIndex(int RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, int AcValueIndex);
        
        [DllImport(DllName)]
        public static extern int PowerGetActiveScheme(int UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport(DllName)]
        public static extern int PowerSetActiveScheme(int UserRootPowerKey, ref Guid SchemeGuid);

        [DllImport(DllName)]
        public static extern int PowerEnumerate(int RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingGuid,
            int AcessFlags, int Index, ref Guid Buffer, ref int BufferSize);

        [DllImport(DllName)]
        public static extern int PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingGuid,
            IntPtr PowerSettingGuid, IntPtr Buffer, ref int BufferSize);

        public static Guid ActiveSchemeGuid
        {
            get
            {
                IntPtr guidPtr = new IntPtr();
                int res = PowerGetActiveScheme(0, ref guidPtr);
                if (res != 0) throw new Win32Exception((int)res);

                Guid ret = (Guid)Marshal.PtrToStructure(guidPtr, typeof(Guid));
                Marshal.FreeHGlobal(guidPtr);

                return ret;
            }
            set
            {
                int res = PowerSetActiveScheme(0, ref value);
                if (res != 0) throw new Win32Exception((int)res);
            }
        }

        public static int ReadPowerSetting(bool ac, ref Guid activeSchemeGuid, ref Guid subGroupGuid, ref Guid settingGuid, ref int value)
        {
            int res = 0;

            if (ac) res = PowerReadACValueIndex(0, ref activeSchemeGuid, ref subGroupGuid, ref settingGuid, ref value);
            else res = PowerReadDCValueIndex(0, ref activeSchemeGuid, ref subGroupGuid, ref settingGuid, ref value);

            if (res != 0) throw new Win32Exception((int)res);

            return res;
        }

        public static int ReadDefaultSetting(bool ac, ref Guid activeSchemeGuid, ref Guid subGroupGuid, ref Guid settingGuid, ref int value)
        {
            int res = 0;

            if (ac) res = PowerReadACDefaultIndex(0, ref activeSchemeGuid, ref subGroupGuid, ref settingGuid, ref value);
            else res = PowerReadDCDefaultIndex(0, ref activeSchemeGuid, ref subGroupGuid, ref settingGuid, ref value);

            if (res != 0) throw new Win32Exception((int)res);
            return res;
        }

        public static int WritePowerSetting(bool ac, ref Guid activeSchemeGuid, ref Guid subGroupGuid, ref Guid settingGuid, int newValue)
        {
            int res = ac ? PowerWriteACValueIndex(0, ref activeSchemeGuid, ref subGroupGuid, ref settingGuid, newValue)
                : PowerWriteDCValueIndex(0, ref activeSchemeGuid, ref subGroupGuid, ref settingGuid, newValue);

            if (res != 0) throw new Win32Exception((int)res);

            res = PowerSetActiveScheme(0, ref activeSchemeGuid);
            if (res != 0) throw new Win32Exception((int)res);

            return res;
        }

        public static Guid BalancedPowerPlan
        {
            get
            {
                Guid subgroup = new Guid("fea3413e-7e05-4911-9a71-700331f1c294"),
                     setting = new Guid("245d8541-3943-4422-b025-13a784f679b7");

                Guid Buffer = new Guid(),
                     BalancedGuid = new Guid();

                int SchemeIndex = 0,
                    BufferSize = (int)Marshal.SizeOf(typeof(Guid));

                while (PowerEnumerate(0, IntPtr.Zero, IntPtr.Zero, 16, SchemeIndex, ref Buffer, ref BufferSize) == 0)
                {
                    int ACvalue = 0,
                        DCvalue = 0;

                    ReadPowerSetting(true, ref Buffer, ref subgroup, ref setting, ref ACvalue);
                    ReadPowerSetting(false, ref Buffer, ref subgroup, ref setting, ref DCvalue);

                    if (ACvalue == 2 && DCvalue == 2)
                        BalancedGuid = Buffer;

                    SchemeIndex++;
                }

                return BalancedGuid;
            }
        }

        public static int CheckPowerSetting(bool ac, Guid guid)
        {
            return PowerSettingAccessCheck(ac ? PowerDataAccessor.ACCESS_AC_POWER_SETTING_INDEX
                                              : PowerDataAccessor.ACCESS_DC_POWER_SETTING_INDEX, guid);
        }

        public static int CheckActiveSchemeAccess
        {
            get
            {
                int res = PowerSettingAccessCheck(PowerDataAccessor.ACCESS_ACTIVE_SCHEME, new Guid());
                if (res != 0) throw new Win32Exception((int)res);
                return res;
            }
        }

        public static bool IsVideoDim
        {
            get
            {
                var powercapabilityes = new SystemPowerCapablities();
                int result = CallNtPowerInformation(4, IntPtr.Zero, 0, ref powercapabilityes, (int)Marshal.SizeOf(powercapabilityes));
                if (result != 0) return false;

                return powercapabilityes.VideoDimPresent == 1;
            }
        }

        public static string ActiveSchemeFriendlyName
        {
            get
            {
                Guid ptrActiveGuid = ActiveSchemeGuid;

                int buffSize = 0;
                int res = PowerReadFriendlyName(IntPtr.Zero, ref ptrActiveGuid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref buffSize);
                if (res == 0)
                {
                    IntPtr ptrName = Marshal.AllocHGlobal((int)buffSize);
                    res = PowerReadFriendlyName(IntPtr.Zero, ref ptrActiveGuid, IntPtr.Zero, IntPtr.Zero, ptrName, ref buffSize);
                    if (res == 0)
                    {
                        string ret = Marshal.PtrToStringUni(ptrName);
                        Marshal.FreeHGlobal(ptrName);
                        return ret;
                    }
                    Marshal.FreeHGlobal(ptrName);
                }

                throw new Win32Exception((int)res);
            }
        }
    }
}
