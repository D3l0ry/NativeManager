using System;

using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IAllocator
    {
        IntPtr Alloc(uint size, AllocationType allocationType = AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection memoryProtection = MemoryProtection.PAGE_EXECUTE_READWRITE);

        bool Protect(IntPtr address, uint size, AllocationProtect protectCode, out AllocationProtect oldProtect);

        bool Free(IntPtr address);
    }
}