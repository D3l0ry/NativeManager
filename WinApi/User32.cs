using System.Runtime.InteropServices;
using System.Security;

namespace System.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static class User32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.UserDll)]
        public static extern IntPtr GetForegroundWindow();
    }
}