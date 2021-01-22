using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe class ModuleManager : SimpleMemoryManager, IMemory
    {
        private IntPtr m_ModulePtr;
        private ProcessModule m_SelectedModule;
        private MEMORY_BASIC_INFORMATION m_ModulePage;

        internal protected ModuleManager(Process process, string moduleName) : base(process)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                m_ModulePtr = IntPtr.Zero;
                m_ModulePage.RegionSize = IntPtr.Zero;

                return;
            }

            m_SelectedModule = m_Process.GetModule(moduleName);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_ModulePtr = m_SelectedModule.BaseAddress;

            m_ModulePage = PageManager.GetPageInformation(process, m_ModulePtr);
        }

        internal protected ModuleManager(Process process, IntPtr modulePtr) : base(process)
        {
            if (modulePtr == IntPtr.Zero)
            {
                m_ModulePtr = modulePtr;
                m_ModulePage.RegionSize = IntPtr.Zero;

                return;
            }

            m_SelectedModule = m_Process.GetModule(modulePtr);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_ModulePtr = m_SelectedModule.BaseAddress;

            m_ModulePage = PageManager.GetPageInformation(process, m_ModulePtr);
        }

        public static implicit operator ProcessModule(ModuleManager moduleManager) => moduleManager.m_SelectedModule;

        public string ModuleName => m_SelectedModule?.ModuleName;

        public IntPtr ModulePtr => m_ModulePtr;

        public override byte[] ReadBytes(IntPtr address, IntPtr size)
        {
            IntPtr newAddress = IntPtr.Add(m_ModulePtr, address.ToInt32());

            if (m_ModulePage.RegionSize != IntPtr.Zero && (newAddress + size.ToInt32()).ToInt64() > m_ModulePage.RegionSize.ToInt64())
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Exceeding the limits of the allocated memory of the module");
            }

            return base.ReadBytes(newAddress, size);
        }

        public override bool WriteBytes(IntPtr address, byte[] buffer)
        {
            IntPtr newAddress = IntPtr.Add(m_ModulePtr, address.ToInt32());

            if (m_ModulePage.RegionSize != IntPtr.Zero && (newAddress + buffer.Length - 1).ToInt64() > m_ModulePage.RegionSize.ToInt64())
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Exceeding the limits of the allocated memory of the module");
            }

            return base.WriteBytes(newAddress, buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address) where T : unmanaged => GenericsConverter.BytesToStructure<T>(this[address, Marshal.SizeOf<T>()]);

        public virtual T[] Read<T>(IntPtr address, int count) where T : unmanaged
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
        public virtual T ReadManaged<T>(IntPtr address) => GenericsConverter.BytesToManaged<T>(this[address, Marshal.SizeOf<T>()]);

        public virtual T[] ReadManaged<T>(IntPtr address, int count)
        {
            int size = Marshal.SizeOf<T>();

            T[] elements = new T[count];

            for (int index = 0; index < count; index++)
            {
                elements[index] = ReadManaged<T>(address + (index * size));
            }

            return elements;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Write<T>(IntPtr address, T value) where T : unmanaged
        {
            IntPtr newAddress = IntPtr.Add(m_ModulePtr, address.ToInt32());
            int valueSize = Marshal.SizeOf<T>();

            if (m_ModulePage.RegionSize != IntPtr.Zero && (newAddress + valueSize).ToInt64() > m_ModulePage.RegionSize.ToInt64())
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Exceeding the limits of the allocated memory of the module");
            }

            return Kernel32.WriteProcessMemory(m_Process.Handle, IntPtr.Add(m_ModulePtr, address.ToInt32()), value, (IntPtr)valueSize, IntPtr.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool WriteManaged<T>(IntPtr address, T value) => WriteBytes(address, GenericsConverter.ManagedToBytes(value));

        IAllocator IMemory.GetAllocator()
        {
            throw new NotImplementedException();
        }
    }
}