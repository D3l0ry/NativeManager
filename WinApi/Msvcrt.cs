using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NativeManager.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class Msvcrt
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr MemCopy(byte[] dest, byte[] src, UIntPtr count);
    }
}