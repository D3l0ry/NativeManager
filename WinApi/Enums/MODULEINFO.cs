using System;
using System.Runtime.InteropServices;

namespace NativeManager.WinApi.Enums
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MODULEINFO
    {
        public IntPtr lpBaseOfDll;
        public long SizeOfImage;
        public IntPtr EntryPoint;
    }
}