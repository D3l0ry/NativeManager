using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.MemoryInteractions
{
    /// <summary>
    /// Предоставляет доступ к адресам выбранного модуля
    /// </summary>
    public unsafe class ModuleManager : SimpleMemoryManager
    {
        private readonly ProcessModule _selectedModule;

        protected internal ModuleManager(Process process, ProcessModule selectedModule) : base(process)
        {
            if (selectedModule == null)
            {
                throw new ArgumentNullException(nameof(selectedModule));
            }

            _selectedModule = selectedModule;
        }

        public ProcessModule Module => _selectedModule;

        public override byte[] ReadBytes(IntPtr address, uint size)
        {
            IntPtr newAddress = TryGetNewAddress(address, size);

            return base.ReadBytes(newAddress, size);
        }

        public override void WriteBytes(IntPtr address, byte[] buffer)
        {
            IntPtr newAddress = TryGetNewAddress(address, (uint)buffer.Length);

            base.WriteBytes(newAddress, buffer);
        }

        /// <summary>
        /// Читает данные по определенному адресу и преобразует их в выбранный тип
        /// </summary>
        /// <param name="address">Адрес, с которого нужно начать чтение</param>
        /// <typeparam name="T">Тип, в который нужно преобразовать массив байт</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address)
        {
            uint size = (uint)Marshal.SizeOf<T>();
            byte[] bytes = ReadBytes(address, size);

            return GenericsConverter.BytesToManaged<T>(bytes);
        }

        /// <summary>
        /// Читает данные по определенному адресу и преобразует их в массив элементов выбранного типа
        /// </summary>
        /// <param name="address">Адрес, с которого нужно начать чтение</param>
        /// <param name="count">Количество  элементов для чтения</param>
        /// <typeparam name="T">Тип, в который нужно преобразовать массив байт</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T[] Read<T>(IntPtr address, uint count)
        {
            int size = Marshal.SizeOf<T>();
            T[] elements = new T[count];

            for (uint index = 0; index < count; index++)
            {
                IntPtr newAddress = (IntPtr)(address.ToInt64() + (index * size));

                elements[index] = Read<T>(newAddress);
            }

            return elements;
        }

        /// <summary>
        /// Записывает данные по определенному адресу
        /// </summary>
        /// <param name="address">Адрес, с которого нужно начать чтение</param>
        /// <param name="value">Значение, которое нужно записать</param>
        /// <typeparam name="T">значение, которое нужно записать</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write<T>(IntPtr address, T value) => WriteBytes(address, GenericsConverter.ManagedToBytes(value));

        protected virtual IntPtr TryGetNewAddress(IntPtr address, uint size)
        {
            IntPtr newAddress = IntPtr.Add(_selectedModule.BaseAddress, address.ToInt32());
            long newAddressAddSize = newAddress.ToInt64() + size;
            long maxAddress = (_selectedModule.BaseAddress + _selectedModule.ModuleMemorySize).ToInt64();

            if (newAddress.ToInt64() < _selectedModule.BaseAddress.ToInt64())
            {
                throw _Process.ShowException<OutOfMemoryException>(newAddress, "Указанный адрес меньше базового адреса модуля");
            }

            if (newAddressAddSize > maxAddress)
            {
                throw _Process.ShowException<OutOfMemoryException>(newAddress, "Указанный адрес больше базового адреса модуля");
            }

            return newAddress;
        }
    }
}