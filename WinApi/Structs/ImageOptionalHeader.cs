using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageOptionalHeader
    {
        public readonly short Magic;
        public readonly byte MajorLinkedVersion;
        public readonly byte MinorLinkedVersion;
        public readonly int SizeOfCode;
        public readonly int SizeOfInitializedData;
        public readonly int SizeOfUInitializedData;
        public readonly int AddressOfEntryPoint;
        public readonly int BaseOfCode;
        public readonly int BaseOfData;
        public readonly int ImageBase;
        public readonly int SectionAlignment;
        public readonly int FileAlignment;
        public readonly short MajorOperatingSystemVersion;
        public readonly short MinororOperatingSystemVersion;
        public readonly short MajorImageVersion;
        public readonly short MinorImageVersion;
        public readonly short MajorSubsystemVersion;
        public readonly short MinorSubsystemVersion;
        public readonly int Win32VersionValue;
        public readonly int SizeOfImage;
        public readonly int SizeOfHeaders;
        public readonly int CheckSum;
        public readonly short Subsystem;
        public readonly short DllCharacteristics;
        public readonly int SizeOfStackReverse;
        public readonly int SizeOfStackCommit;
        public readonly int SizeOfHeapReserve;
        public readonly int SizeOfHeapCommit;
        public readonly int LoaderFlags;
        public readonly int NumberOfRvaAndSizes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ImageDataDirectory[] DataDirectory;
    }
}