using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.MemoryInteraction
{
    public class ModuleManager : SimpleMemoryManager,IMemory
    {
        private IntPtr m_ModulePtr;

        internal ModuleManager(Process process, string moduleName):base(process)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            ProcessModule selectedModule = m_Process.GetModule(moduleName);

            if (selectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_ModulePtr = selectedModule.BaseAddress;
        }

        public override byte[] ReadBytes(IntPtr address, IntPtr size) => base.ReadBytes(IntPtr.Add(m_ModulePtr, address.ToInt32()), size);

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
        public virtual bool Write<T>(IntPtr address, T value) where T : unmanaged => WriteBytes(IntPtr.Add(m_ModulePtr, address.ToInt32()), GenericsConverter.StructureToBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool WriteManaged<T>(IntPtr address, T value) => WriteBytes(address, GenericsConverter.ManagedToBytes(value));

        IAllocator IMemory.GetAllocator()
        {
            throw new NotImplementedException();
        }
    }
}