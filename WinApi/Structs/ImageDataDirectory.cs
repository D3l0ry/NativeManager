using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageDataDirectory
    {
        public int VirtualAddress;
        public int Size;
    }
}