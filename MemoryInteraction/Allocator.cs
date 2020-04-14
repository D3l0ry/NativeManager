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

        public Allocator(IMemory memory) => m_Memory = memory;

        public IntPtr Alloc(uint size, AllocationType allocationType = AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection memoryProtection = MemoryProtection.PAGE_EXECUTE_READWRITE) => Kernel32.VirtualAllocEx(m_Memory.Handle, IntPtr.Zero, size, allocationType, memoryProtection);

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(m_Memory.Handle, address, 0, FreeType.MEM_RELEASE);

        public bool Protect(IntPtr address, uint size, AllocationProtect protectCode, out AllocationProtect oldProtect) => Kernel32.VirtualProtectEx(m_Memory.Handle, address, (UIntPtr)size, protectCode, out oldProtect);
    }
}