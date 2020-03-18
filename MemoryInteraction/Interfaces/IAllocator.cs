using System;

using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IAllocator
    {
        IntPtr Alloc(uint size);

        bool Protect(IntPtr address, uint size, ProtectCode protectCode, out ProtectCode oldProtect);

        bool Free(IntPtr address);
    }
}