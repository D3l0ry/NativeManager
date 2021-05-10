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

        public bool Execute(IntPtr address, IntPtr args)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(m_Process.Handle, IntPtr.Zero, IntPtr.Zero, address, args, IntPtr.Zero, IntPtr.Zero);

            Kernel32.WaitForSingleObject(thread, 0xFFFFFFFF);
            Kernel32.CloseHandle(thread);

            return thread != IntPtr.Zero;
        }

        public static IntPtr GetFunction(IntPtr address, string functionName)
        {
            if (string.IsNullOrWhiteSpace(functionName)) throw new ArgumentNullException(nameof(functionName));

            return Kernel32.GetProcAddress(address, functionName);
        }

        public static IntPtr GetFunction(string moduleName, string functionName)
        {
            if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentNullException(nameof(moduleName));

            return GetFunction(Kernel32.GetModuleHandle(moduleName), functionName);
        }
    }
}