using System;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public class Allocator : IAllocator
    {
        #region Private variables
        private readonly IMemory m_Memory;
        #endregion

        public Allocator(IMemory allocator) => m_Memory = allocator;

        public IntPtr Alloc(uint size, AllocationType allocationType = AllocationType.Commit | AllocationType.Reserve, MemoryProtection memoryProtection = MemoryProtection.ExecuteReadWrite) => Kernel32.VirtualAllocEx(m_Memory.Handle, IntPtr.Zero, size, allocationType, memoryProtection);

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(m_Memory.Handle, address, 0, AllocationType.Release);

        public bool Protect(IntPtr address, uint size, ProtectCode protectCode, out ProtectCode oldProtect) => Kernel32.VirtualProtectEx(m_Memory.Handle, address, (UIntPtr)size, protectCode, out oldProtect);
    }
}