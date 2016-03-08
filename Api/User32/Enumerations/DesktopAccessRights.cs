namespace ManagedWin32.Api
{
    public enum DesktopAccessRights
    {
        CreateWindow = 0x0002,
        Enumerate = 0x0040,
        WriteObjects = 0x0080,
        SwitchDesktop = 0x0100,
        CreateMenu = 0x0004,
        HookControl = 0x0008,
        ReadObjects = 0x0001,
        JournalRecord = 0x0010,
        JournalPlayback = 0x0020,
        AllRights = JournalRecord | JournalPlayback | CreateWindow
            | Enumerate | WriteObjects | SwitchDesktop
            | CreateMenu | HookControl | ReadObjects
    }
}