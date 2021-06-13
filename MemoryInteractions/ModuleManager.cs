using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteraction
{
    /// <summary>
    /// Предоставляет доступ к адресам выбранного модуля
    /// </summary>
    public unsafe class ModuleManager : SimpleMemoryManager, IMemory
    {
        private readonly IntPtr m_Address;
        private ProcessModule m_SelectedModule;

        internal protected ModuleManager(Process process, string moduleName) :base(process)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                m_Address = IntPtr.Zero;

                return;
            }

            m_SelectedModule = m_Process.GetModule(moduleName);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_Address = m_SelectedModule.BaseAddress;
        }

        internal protected ModuleManager(Process process, IntPtr modulePtr) : base(process)
        {
            if (modulePtr == IntPtr.Zero)
            {
                m_Address = modulePtr;

                return;
            }

            m_SelectedModule = m_Process.GetModule(modulePtr);

            if (m_SelectedModule is null)
            {
                throw new NullReferenceException("Module not found");
            }

            m_Address = m_SelectedModule.BaseAddress;
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

        /// <summary>
        /// Читает данные по определенному адресу и преобразует их в выбранный тип
        /// </summary>
        /// <param name="address">Адрес, с которого нужно начать чтение</param>
        /// <typeparam name="T">Тип, в который нужно преобразовать массив байт</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address) => GenericsConverter.BytesToManaged<T>(ReadBytes(address, Marshal.SizeOf<T>()));

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

        private IntPtr TryGetNewAddress(IntPtr address, int size)
        {
            if (m_SelectedModule != null)
            {
                IntPtr newAddress = IntPtr.Add(m_Address, address.ToInt32());

                if (((newAddress + size).ToInt64() > (m_Address + m_SelectedModule.ModuleMemorySize).ToInt64()))
                {
                    throw new ArgumentOutOfRangeException(nameof(address), $"Exceeding the limits of the allocated memory of the module");
                }

                return newAddress;
            }

            return address;
        }
    }
}