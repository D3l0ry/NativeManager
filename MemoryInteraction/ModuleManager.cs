using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe class ModuleManager : SimpleMemoryManager,IMemory
    {
        private IntPtr m_ModulePtr;
        private ProcessModule m_SelectedModule;

        internal ModuleManager(Process process, string moduleName) : base(process)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                m_ModulePtr = IntPtr.Zero;

                return;
            }

            m_SelectedModule = m_Process.GetModule(moduleName);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_ModulePtr = m_SelectedModule.BaseAddress;
        }

        internal ModuleManager(Process process, IntPtr modulePtr) : base(process)
        {
            if (modulePtr == IntPtr.Zero)
            {
                m_ModulePtr = modulePtr;

                return;
            }

            m_SelectedModule = m_Process.GetModule(modulePtr);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_ModulePtr = m_SelectedModule.BaseAddress;
        }

        public static implicit operator ProcessModule(ModuleManager moduleManager) => moduleManager.m_SelectedModule;

        public string ModuleName => m_SelectedModule?.ModuleName;

        public IntPtr ModulePtr => m_ModulePtr;

        public override byte[] ReadBytes(IntPtr address, IntPtr size) => base.ReadBytes(IntPtr.Add(m_ModulePtr, address.ToInt32()), size);

        public override bool WriteBytes(IntPtr address, byte[] buffer) => base.WriteBytes(IntPtr.Add(m_ModulePtr, address.ToInt32()), buffer);

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
        public virtual bool Write<T>(IntPtr address, T value) where T : unmanaged => Kernel32.WriteProcessMemory(m_Process.Handle, IntPtr.Add(m_ModulePtr, address.ToInt32()), value, (IntPtr)Marshal.SizeOf<T>(), IntPtr.Zero);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool WriteManaged<T>(IntPtr address, T value) => WriteBytes(address, GenericsConverter.ManagedToBytes(value));

        IAllocator IMemory.GetAllocator()
        {
            throw new NotImplementedException();
        }
    }
}