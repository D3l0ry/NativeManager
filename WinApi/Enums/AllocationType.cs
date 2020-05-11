using System;

namespace NativeManager.WinApi.Enums
{
    [Flags]
    public enum AllocationType
    {
        MEM_COMMIT = 0x1000,
        MEM_RESERVE = 0x2000,
        MEM_RESET = 0x80000,
        MEM_RESET_UNDO = 0x1000000,
        MEM_PHYSICAL = 0x400000,
        MEM_TOP_DOWN = 0x100000,
        MEM_LARGE_PAGES = 0x20000000
    }
}