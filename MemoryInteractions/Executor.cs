using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteractions
{
    public sealed unsafe class Executor
    {
        private readonly Process m_Process;

        public Executor(Process process)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            m_Process = process;
        }

        /// <summary>
        /// Вызывает функцию по определенному адресу
        /// </summary>
        /// <param name="address">Адрес функции</param>
        /// <param name="args">Аргументы функции</param>
        /// <returns></returns>
        public bool Execute(IntPtr address, IntPtr args)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(m_Process.Handle, IntPtr.Zero, 0, address, args, IntPtr.Zero, IntPtr.Zero);

            if(thread == IntPtr.Zero)
            {
                throw m_Process.ShowException<OverflowException>(address, "Не удалось создать новый поток внутри процесса");
            }

            Kernel32.WaitForSingleObject(thread, 0xFFFFFFFF);
            Kernel32.CloseHandle(thread);

            return thread != IntPtr.Zero;
        }

        /// <summary>
        /// Получает адрес функции
        /// </summary>
        /// <param name="moduleAddress">Адрес модуля, в котором находится функция</param>
        /// <param name="functionName">Имя функции</param>
        /// <returns></returns>
        public static IntPtr GetFunction(IntPtr moduleAddress, string functionName)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                throw new ArgumentNullException(nameof(functionName));
            }

            return Kernel32.GetProcAddress(moduleAddress, functionName);
        }

        /// <summary>
        /// Получает адрес функции
        /// </summary>
        /// <param name="moduleName">Имя модуля, в котором находится функция</param>
        /// <param name="functionName">Имя функции</param>
        /// <returns></returns>
        public static IntPtr GetFunction(string moduleName, string functionName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            IntPtr moduleAddress = Kernel32.GetModuleHandle(moduleName);

            return GetFunction(moduleAddress, functionName);
        }
    }
}