using System.Collections.Generic;
using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    /// <summary>
    /// Предоставляет доступ к чтению и записи виртуальной памяти процесса
    /// </summary>
    public class SimpleMemoryManager : ISimpleMemory
    {
        protected Process m_Process;

        public SimpleMemoryManager(Process process)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            m_Process = process;
        }

        /// <summary>
        /// Читает массив байт по определенному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать чтение</param>
        /// <param name="size">Количество считываемых байт</param>
        /// <returns></returns>
        public virtual byte[] ReadBytes(IntPtr address, IntPtr size)
        {
            byte[] buffer = new byte[size.ToInt32()];

            Kernel32.ReadProcessMemory(m_Process.Handle, address, buffer, size, IntPtr.Zero);

            return buffer;
        }

        /// <summary>
        /// Читает массив байт по определенному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать чтение</param>
        /// <param name="size">Колличество считываемых байт</param>
        /// <returns></returns>
        public virtual byte[] ReadBytes(IntPtr address, int size) => ReadBytes(address, (IntPtr)size);

        /// <summary>
        /// Читает массив байт по определенному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать чтение</param>
        /// <param name="predicate">Заканчивает чтение по достижению указанного условия</param>
        /// <returns></returns>
        public virtual IEnumerable<byte> ReadBytes(IntPtr address, Predicate<byte> predicate)
        {
            List<byte> buffer = new List<byte>();

            int index = 0;
            byte element = ReadBytes(address, 1)[0];

            buffer.Add(element);

            while (true)
            {
                element = ReadBytes(address + ++index, 1)[0];

                if (predicate(element)) break;

                buffer.Add(element);
            }

            return buffer;
        }

        /// <summary>
        /// Записывает массив байт по указанному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать запись</param>
        /// <param name="buffer">Массив байт, которые нужно записать</param>
        /// <returns></returns>
        public virtual bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(m_Process.Handle, address, buffer, (IntPtr)buffer.Length, IntPtr.Zero);

        /// <summary>
        /// Записывает массив байт по указанному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать запись</param>
        /// <param name="buffer">Массив байт, которые нужно записать</param>
        /// <returns></returns>
        public virtual bool WriteBytes(IntPtr address, sbyte[] buffer) => Kernel32.WriteProcessMemory(m_Process.Handle, address, buffer, (IntPtr)buffer.Length, IntPtr.Zero);
    }
}