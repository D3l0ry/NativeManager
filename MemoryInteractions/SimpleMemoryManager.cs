using System.Collections.Generic;
using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteractions
{
    /// <summary>
    /// Предоставляет доступ к чтению и записи виртуальной памяти процесса
    /// </summary>
    public class SimpleMemoryManager
    {
        protected readonly Process _Process;

        public SimpleMemoryManager(Process process)
        {
            ProcessExtensions.CheckProcess(process);

            _Process = process;
        }

        /// <summary>
        /// Читает массив байт по определенному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать чтение</param>
        /// <param name="size">Количество считываемых байт</param>
        /// <returns></returns>
        public virtual byte[] ReadBytes(IntPtr address, uint size)
        {
            byte[] buffer = new byte[size];
            bool readResult = Kernel32.ReadProcessMemory(_Process.Handle, address, buffer, size, IntPtr.Zero);

            if (!readResult)
            {
                throw _Process.ShowException<AccessViolationException>(address, $"Не удалось прочитать массив байт по адресу {address}");
            }

            return buffer;
        }

        /// <summary>
        /// Читает массив байт по определенному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать чтение</param>
        /// <param name="predicate">Заканчивает чтение по достижению указанного условия</param>
        /// <returns></returns>
        public virtual byte[] ReadBytes(IntPtr address, Predicate<byte> predicate)
        {
            List<byte> buffer = new List<byte>();

            while (true)
            {
                byte element = ReadBytes(address, 1)[0];

                if (!predicate(element))
                {
                    break;
                }

                address += 1;

                buffer.Add(element);
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Записывает массив байт по указанному адресу
        /// </summary>
        /// <param name="address">Адрес с которого начать запись</param>
        /// <param name="buffer">Массив байт, которые нужно записать</param>
        /// <returns></returns>
        public virtual void WriteBytes(IntPtr address, byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            bool writeResult = Kernel32.WriteProcessMemory(_Process.Handle, address, buffer, (uint)buffer.Length, IntPtr.Zero);

            if (writeResult)
            {
                return;
            }

            throw _Process.ShowException<AccessViolationException>(address, $"Не удалось записать массив байт по адресу {address}");
        }
    }
}