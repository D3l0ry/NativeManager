using System;

using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IAllocator
    {
        IntPtr Alloc(uint size, AllocationType allocationType = AllocationType.Commit | AllocationType.Reserve, MemoryProtection memoryProtection = MemoryProtection.ExecuteReadWrite);

        bool Protect(IntPtr address, uint size, AllocationProtect protectCode, out AllocationProtect oldProtect);

        bool Free(IntPtr address);
    }
}