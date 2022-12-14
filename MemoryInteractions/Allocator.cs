using System.Diagnostics;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteractions
{
    /// <summary>
    /// Предоставляет доступ к выделению и управлению правами виртуальной памяти процесса
    /// </summary>
    public sealed class Allocator
    {
        private readonly Process m_Process;

        public Allocator(Process process)
        {
            ProcessExtensions.CheckProcess(process);

            m_Process = process;
        }

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(uint size, AllocationType allocationType, MemoryProtection memoryProtection)
        {
            IntPtr allocationAddress = Kernel32
                .VirtualAllocEx(m_Process.Handle, IntPtr.Zero, size, allocationType, memoryProtection);

            if (allocationAddress == IntPtr.Zero)
            {
                throw new OverflowException("Не удалось выделить область в памяти процесса");
            }

            return allocationAddress;
        }

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(uint size) => Alloc(size, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, MemoryProtection.PAGE_EXECUTE_READWRITE);

        /// <summary>
        /// Выделяет память и получает указатель на выделенную память
        /// </summary>
        /// <returns></returns>
        public IntPtr Alloc(uint size, AllocationType allocationType) => Alloc(size, allocationType, MemoryProtection.PAGE_EXECUTE_READWRITE);

        /// <summary>
        /// Указывает, что данные в диапазоне памяти, указанном address и size, больше не представляют интереса
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IntPtr Reset(IntPtr address, uint size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, size, AllocationType.MEM_RESET, MemoryProtection.PAGE_NOACCESS);

        /// <summary>
        /// Указывает на то, что данные в указанном диапазоне памяти, заданном lpAddress и dwSize, представляют интерес для вызывающего абонента и пытаются обратить вспять эффекты MEM_RESET
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IntPtr Undo(IntPtr address, uint size) => Kernel32.VirtualAllocEx(m_Process.Handle, address, size, AllocationType.MEM_RESET_UNDO, MemoryProtection.PAGE_NOACCESS);

        /// <summary>
        /// Освобождает выделенную память
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(m_Process.Handle, address, 0, FreeType.MEM_RELEASE);

        /// <summary>
        /// Изменяет права определенного участка памяти
        /// </summary>
        /// <param name="address">Адрес с которого нужно изменить права</param>
        /// <param name="size">Размер блока памяти</param>
        /// <param name="protectCode">Новые права для блока памяти</param>
        /// <returns>Старые права участка памяти</returns>
        public AllocationProtect Protect(IntPtr address, uint size, AllocationProtect protectCode)
        {
            bool protectResult = Kernel32
                .VirtualProtectEx(m_Process.Handle, address, size, protectCode, out AllocationProtect oldProtect);

            if (!protectResult)
            {
                throw m_Process.ShowException<OverflowException>(address, $"Не удалось изменить права выбранного участка памяти по адресу {address}");
            }

            return oldProtect;
        }

        /// <summary>
        /// Изменяет права определенного участка памяти
        /// </summary>
        /// <param name="address">Адрес с которого нужно изменить права</param>
        /// <param name="protectCode">Новые права для блока памяти</param>
        /// <typeparam name="T">Тип, с помощью которого вычисляется размер памяти, которую нужно изменить</typeparam>
        /// <returns></returns>
        public AllocationProtect Protect<T>(IntPtr address, AllocationProtect protectCode) => Protect(address, (uint)Marshal.SizeOf<T>(), protectCode);
    }
}