using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageExportDirectory
    {
        public readonly int Characteristics;
        public readonly int TimeDateStamp;
        public readonly short MajorVersion;
        public readonly short MinorVersion;
        public readonly int Name;
        public readonly int Base;
        public int NumberOfFunctions;
        public int NumberOfNames;
        public int AddressOfFunctions;
        public int AddressOfNames;
        public int AddressOfNameOrdinals;
    }
}