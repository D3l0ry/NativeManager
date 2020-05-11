using System;
using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public class Allocator : IAllocator
    {
        private readonly IMemory m_Memory;

        public Allocator(IMemory memory) => m_Memory = memory;

        public IntPtr Alloc(uint size, MemoryProtection memoryProtection = MemoryProtection.PAGE_EXECUTE_READWRITE) => Kernel32.VirtualAllocEx(m_Memory.SelectedProcess.Handle, IntPtr.Zero, size, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, memoryProtection);

        public IntPtr Reset(IntPtr address, uint size) => Kernel32.VirtualAllocEx(m_Memory.SelectedProcess.Handle, address, size, AllocationType.MEM_RESET, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Undo(IntPtr address, uint size) => Kernel32.VirtualAllocEx(m_Memory.SelectedProcess.Handle, address, size, AllocationType.MEM_RESET_UNDO, MemoryProtection.PAGE_NOACCESS);

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(m_Memory.SelectedProcess.Handle, address, 0, FreeType.MEM_RELEASE);

        public bool Protect(IntPtr address, uint size, AllocationProtect protectCode, out AllocationProtect oldProtect) => Kernel32.VirtualProtectEx(m_Memory.SelectedProcess.Handle, address, (UIntPtr)size, protectCode, out oldProtect);
    }
}