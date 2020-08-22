using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public sealed class Allocator : IAllocator
    {
        private Process m_Process;

        public Allocator(Process process) => m_Process = process;

        public IntPtr Alloc(int size) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, (IntPtr)size, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection.PAGE_EXECUTE_READWRITE);

        public IntPtr Alloc(IntPtr size) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, size, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection.PAGE_EXECUTE_READWRITE);

        public IntPtr Reset(IntPtr address, int size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, (IntPtr)size, AllocationType.MEM_RESET, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Reset(IntPtr address, IntPtr size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, size, AllocationType.MEM_RESET, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Undo(IntPtr address, int size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, (IntPtr)size, AllocationType.MEM_RESET_UNDO, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Undo(IntPtr address, IntPtr size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, size, AllocationType.MEM_RESET_UNDO, MemoryProtection.PAGE_NOACCESS);

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(m_Process.Handle, address, IntPtr.Zero, FreeType.MEM_RELEASE);

        public bool Protect(IntPtr address, int size, AllocationProtect protectCode, out AllocationProtect oldProtect) => Kernel32.VirtualProtectEx(m_Process.Handle, address, (IntPtr)size, protectCode, out oldProtect);

        public bool Protect(IntPtr address, IntPtr size, AllocationProtect protectCode, out AllocationProtect oldProtect) => Kernel32.VirtualProtectEx(m_Process.Handle, address, size, protectCode, out oldProtect);

        public void Dispose()
        {
            m_Process = null;

            GC.SuppressFinalize(this);
        }
    }
}