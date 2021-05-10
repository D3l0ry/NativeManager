﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe class ModuleManager : SimpleMemoryManager, IMemory
    {
        private readonly IntPtr m_Address;
        private ProcessModule m_SelectedModule;
        private MEMORY_BASIC_INFORMATION m_ModulePage;

        internal protected ModuleManager(Process process, string moduleName) :base(process)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                m_ModulePage.RegionSize = IntPtr.Zero;

                return;
            }

            m_SelectedModule = m_Process.GetModule(moduleName);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_Address = m_SelectedModule.BaseAddress;

            m_ModulePage = PageManager.GetPageInformation(process, m_Address);
        }

        internal protected ModuleManager(Process process, IntPtr modulePtr) : base(process)
        {
            if (modulePtr == IntPtr.Zero)
            {
                m_Address = modulePtr;
                m_ModulePage.RegionSize = IntPtr.Zero;

                return;
            }

            m_SelectedModule = m_Process.GetModule(modulePtr);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_Address = m_SelectedModule.BaseAddress;

            m_ModulePage = PageManager.GetPageInformation(process, m_Address);
        }

        public static implicit operator ProcessModule(ModuleManager moduleManager) => moduleManager.m_SelectedModule;

        public string ModuleName => m_SelectedModule?.ModuleName;

        public IntPtr ModulePtr => m_Address;

        public override byte[] ReadBytes(IntPtr address, IntPtr size)
        {
            IntPtr newAddress = TryGetNewAddress(address, size.ToInt32());

            return base.ReadBytes(newAddress, size);
        }

        public override bool WriteBytes(IntPtr address, byte[] buffer)
        {
            IntPtr newAddress = TryGetNewAddress(address, buffer.Length);

            return base.WriteBytes(newAddress, buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address) => GenericsConverter.BytesToManaged<T>(ReadBytes(address, Marshal.SizeOf<T>()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T[] Read<T>(IntPtr address, int count)
        {
            int size = Marshal.SizeOf<T>();

            T[] elements = new T[count];

            for (int index = 0; index < count; index++)
            {
                elements[index] = Read<T>(address + (index * size));
            }

            return elements;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Write<T>(IntPtr address, T value) => WriteBytes(address, GenericsConverter.ManagedToBytes(value));

        private IntPtr TryGetNewAddress(IntPtr address, int size)
        {
            IntPtr newAddress = IntPtr.Add(m_Address, address.ToInt32());

            if (m_ModulePage.RegionSize != IntPtr.Zero && (newAddress + size).ToInt64() > m_ModulePage.RegionSize.ToInt64())
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Exceeding the limits of the allocated memory of the module");
            }

            return newAddress;
        }
    }
}