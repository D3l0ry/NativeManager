using NativeManager.WinApi.Enums;

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NativeManager.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static class User32
    {
        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(KeysCode vKey);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
}