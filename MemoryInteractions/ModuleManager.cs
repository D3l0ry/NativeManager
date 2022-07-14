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
        private readonly ProcessModule m_SelectedModule;

        protected internal ModuleManager(Process process, ProcessModule selectedModule) : base(process)
        {
            if (selectedModule is null)
            {
                throw new ArgumentNullException(nameof(selectedModule));
            }

            m_SelectedModule = selectedModule;
        }

        public static implicit operator ProcessModule(ModuleManager moduleManager) => moduleManager.m_SelectedModule;

        public ProcessModule Module => m_SelectedModule;

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

        /// <summary>
        /// Читает данные по определенному адресу и преобразует их в выбранный тип
        /// </summary>
        /// <param name="address">Адрес, с которого нужно начать чтение</param>
        /// <typeparam name="T">Тип, в который нужно преобразовать массив байт</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address)
        {
            byte[] bytes = ReadBytes(address, Marshal.SizeOf<T>());

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

        /// <summary>
        /// Записывает данные по определенному адресу
        /// </summary>
        /// <param name="address">Адрес, с которого нужно начать чтение</param>
        /// <param name="value">Значение, которое нужно записать</param>
        /// <typeparam name="T">значение, которое нужно записать</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Write<T>(IntPtr address, T value) => WriteBytes(address, GenericsConverter.ManagedToBytes(value));

        protected virtual IntPtr TryGetNewAddress(IntPtr address, int size)
        {
            IntPtr newAddress = IntPtr.Add(m_SelectedModule.BaseAddress, address.ToInt32());
            long newAddressAddSize = (newAddress + size).ToInt64();
            long maxAddress = (m_SelectedModule.BaseAddress + m_SelectedModule.ModuleMemorySize).ToInt64();

            if (newAddress.ToInt64() < m_SelectedModule.BaseAddress.ToInt64())
            {
                throw new OutOfMemoryException("Указанный адрес меньше адреса базового адреса модуля");
            }

            if (newAddressAddSize > maxAddress)
            {
                throw new OutOfMemoryException("Указанный адрес больше максимального адреса модуля");
            }

            return newAddress;
        }
    }
}