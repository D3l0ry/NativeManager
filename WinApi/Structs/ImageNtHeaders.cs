using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageNtHeaders
    {
        public readonly int Signature;
        public readonly ImageFileHeader FileHeader;
        public ImageOptionalHeader OptionalHeader;
    }
}