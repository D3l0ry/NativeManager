using System;
using System.Runtime.InteropServices;

namespace NativeManager.WinApi.Enums
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_INFO
    {
        internal ushort wProcessorArchitecture;
        internal ushort wReserved;
        internal uint dwPageSize;
        internal IntPtr lpMinimumApplicationAddress;
        internal IntPtr lpMaximumApplicationAddress;
        internal IntPtr dwActiveProcessorMask;
        internal uint dwNumberOfProcessors;
        internal uint dwProcessorType;
        internal uint dwAllocationGranularity;
        internal ushort wProcessorLevel;
        internal ushort wProcessorRevision;
    }
}