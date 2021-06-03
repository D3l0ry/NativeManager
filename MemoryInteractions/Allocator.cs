using System.Diagnostics;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteraction
{
    /// <summary>
    /// Предоставляет доступ к выделению и управлению правами виртуальной памяти процесса
    /// </summary>
    public sealed class Allocator
    {
        private Process m_Process;

        public Allocator(Process process) => m_Process = process;

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(int size) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, (IntPtr)size, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection.PAGE_EXECUTE_READWRITE);

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(IntPtr size) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, size, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection.PAGE_EXECUTE_READWRITE);

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(int size, AllocationType allocationType) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, (IntPtr)size, allocationType, MemoryProtection.PAGE_EXECUTE_READWRITE);

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(IntPtr size, AllocationType allocationType) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, size, allocationType, MemoryProtection.PAGE_EXECUTE_READWRITE);

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(int size, AllocationType allocationType, MemoryProtection memoryProtection) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, (IntPtr)size, allocationType, memoryProtection);

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(IntPtr size, AllocationType allocationType, MemoryProtection memoryProtection) => Kernel32.VirtualAllocEx(m_Process.Handle, IntPtr.Zero, size, allocationType, memoryProtection);

        public IntPtr Reset(IntPtr address, int size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, (IntPtr)size, AllocationType.MEM_RESET, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Reset(IntPtr address, IntPtr size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, size, AllocationType.MEM_RESET, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Undo(IntPtr address, int size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, (IntPtr)size, AllocationType.MEM_RESET_UNDO, MemoryProtection.PAGE_NOACCESS);

        public IntPtr Undo(IntPtr address, IntPtr size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, size, AllocationType.MEM_RESET_UNDO, MemoryProtection.PAGE_NOACCESS);

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(m_Process.Handle, address, IntPtr.Zero, FreeType.MEM_RELEASE);

        /// <summary>
        /// Изменяет права определенного участка памяти
        /// </summary>
        /// <param name="address">Адрес с которого нужно изменить права</param>
        /// <param name="size">Размер блока памяти</param>
        /// <param name="protectCode">Новые права для блока памяти</param>
        /// <returns></returns>
        public AllocationProtect Protect(IntPtr address, IntPtr size, AllocationProtect protectCode)
        {
            Kernel32.VirtualProtectEx(m_Process.Handle, address, size, protectCode, out AllocationProtect oldProtect);

            return oldProtect;
        }

        /// <summary>
        /// Изменяет права определенного участка памяти
        /// </summary>
        /// <param name="address">Адрес с которого нужно изменить права</param>
        /// <param name="size">Размер блока памяти</param>
        /// <param name="protectCode">Новые права для блока памяти</param>
        /// <returns></returns>
        public AllocationProtect Protect(IntPtr address, int size, AllocationProtect protectCode) => Protect(address, (IntPtr)size, protectCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">Адрес с которого нужно изменить права</param>
        /// <param name="protectCode">Новые права для блока памяти</param>
        /// <typeparam name="T">Тип, с помощью которого вычисляется размер памяти, которую нужно изменить</typeparam>
        /// <returns></returns>
        public AllocationProtect Protect<T>(IntPtr address, AllocationProtect protectCode) => Protect(address, Marshal.SizeOf<T>(), protectCode);
    }
}