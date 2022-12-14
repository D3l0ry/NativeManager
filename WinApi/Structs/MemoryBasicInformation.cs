using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public AllocationProtect AllocationProtect;
        public IntPtr RegionSize;
        public StateType State;
        public AllocationProtect Protect;
        public MemoryType Type;
    }
}