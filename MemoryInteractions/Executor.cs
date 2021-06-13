using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe sealed class Executor
    {
        private Process m_Process;

        public Executor(Process process)
        {
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
            IntPtr thread = Kernel32.CreateRemoteThread(m_Process.Handle, IntPtr.Zero, IntPtr.Zero, address, args, IntPtr.Zero, IntPtr.Zero);

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
            if (string.IsNullOrWhiteSpace(functionName)) throw new ArgumentNullException(nameof(functionName));

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
            if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentNullException(nameof(moduleName));

            return GetFunction(Kernel32.GetModuleHandle(moduleName), functionName);
        }
    }
}