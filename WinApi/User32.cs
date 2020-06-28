using System.Runtime.InteropServices;
using System.Security;

namespace System.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static class User32
    {
        private const string m_UserDll = "user32.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_UserDll)]
        public static extern IntPtr GetForegroundWindow();
    }
}