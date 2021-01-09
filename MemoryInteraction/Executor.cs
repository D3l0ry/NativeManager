using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe sealed class Executor
    {
        private Process m_Process;
        private IMemory m_Memory;

        public Executor(Process process, IMemory memory)
        {
            m_Process = process;
            m_Memory = memory;
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

        public bool CallFunction(IntPtr address, byte[] args)
        {
            IntPtr alloc = IntPtr.Zero;
            bool execute = false;

            try
            {
                alloc = m_Memory.GetAllocator().Alloc(args.Length);

                if (alloc == IntPtr.Zero) return execute;

                if (!m_Memory.WriteBytes(alloc, args)) return execute;

                execute = Execute(address, alloc);
            }
            finally
            {
                m_Memory.GetAllocator().Free(alloc);
            }

            return execute;
        }

        public bool CallFunction<T>(IntPtr address, ref T args) => CallFunction(address, GenericsConverter.ManagedToBytes(args));

        public bool CallFunction<T>(IntPtr address, T args) => CallFunction(address, GenericsConverter.ManagedToBytes(args));

        public void Dispose()
        {
            m_Process = null;
            m_Memory = null;

            GC.SuppressFinalize(this);
        }
    }
}