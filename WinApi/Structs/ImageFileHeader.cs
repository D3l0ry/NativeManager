using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct ImageFileHeader
    {
        public readonly short Machine;
        public readonly short NumberOfSections;
        public readonly int TimeDataStamp;
        public readonly int PointerToSymbolTable;
        public readonly int NumberOfSymbols;
        public readonly short SizeOfOptionalHeader;
        public readonly short Characteristics;
    }
}