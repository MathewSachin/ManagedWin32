using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum ShutdownFlags
    {
        //Shuts down all processes running in the logon session of the process that called the ExitWindowsEx function. 
        //Then it logs the user off.
        //This flag can be used only by processes running in an interactive user's logon session.
        LogOff = 0x00000000,

        //Shuts down the system to a point at which it is safe to turn off the power. 
        //All file buffers have been flushed to disk, and all running processes have stopped. 
        Shutdown = 0x00000001,

        //Shuts down the system and then restarts the system. 
        Reboot = 0x00000002,

        //This flag has no effect if terminal services is enabled. 
        //Otherwise, the system does not send the WM_QUERYENDSESSION message. 
        //This can cause applications to lose data. 
        //Therefore, you should only use this flag in an emergency.
        Force = 0x00000004,

        //Shuts down the system and turns off the power. The system must support the power-off feature. 
        PowerOff = 0x00000008,

        //Forces processes to terminate if they do not respond to the WM_QUERYENDSESSION or WM_ENDSESSION message within the timeout interval.
        ForceIfHung = 0x00000010,

        //Shuts down the system and then restarts it, 
        //as well as any applications that have been registered for restart 
        //using the RegisterApplicationRestart function.
        //These application receive the WM_QUERYENDSESSION message with lParam 
        //set to the ENDSESSION_CLOSEAPP value.
        RestartApps = 0x00000040,

        //Beginning with Windows 8:  You can prepare the system for a faster startup 
        //by combining the EWX_HYBRID_SHUTDOWN flag with the EWX_SHUTDOWN flag. 
        HybridShutdown = 0x00400000
    }
}