using System;

using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IAllocator
    {
        IntPtr Alloc(uint size, MemoryProtection memoryProtection = MemoryProtection.PAGE_EXECUTE_READWRITE);

        IntPtr Reset(IntPtr address, uint size);

        IntPtr Undo(IntPtr address, uint size);

        bool Protect(IntPtr address, uint size, AllocationProtect protectCode, out AllocationProtect oldProtect);

        bool Free(IntPtr address);
    }
}