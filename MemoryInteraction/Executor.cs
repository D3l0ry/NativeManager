using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe sealed class Executor
    {
        private readonly Process m_Process;
        private readonly IMemory m_Memory;

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
            if(string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentNullException(nameof(moduleName));

            return GetFunction(Kernel32.GetModuleHandle(moduleName), functionName);
        }

        public bool CallFunction(IntPtr address, ref byte[] args)
        {
            IntPtr alloc = IntPtr.Zero;
            bool execute = false;

            try
            {
                alloc = m_Memory.GetAllocator().Alloc(args.Length);

                if (alloc == IntPtr.Zero) return false;

                if (!m_Memory.WriteBytes(alloc, args)) return false;

                execute = Execute(address, alloc);
            }
            finally
            {
                m_Memory.GetAllocator().Free(alloc);
            }

            return execute;
        }

        public bool CallFunction(IntPtr address, byte[] args)
        {
            IntPtr alloc = IntPtr.Zero;
            bool execute = false;

            try
            {
                alloc = m_Memory.GetAllocator().Alloc(args.Length);

                if (alloc == IntPtr.Zero) return false;

                if (!m_Memory.WriteBytes(alloc, args)) return false;

                execute = Execute(address, alloc);
            }
            finally
            {
                m_Memory.GetAllocator().Free(alloc);
            }

            return execute;
        }

        public bool CallFunction<T>(IntPtr address, ref T args) where T : unmanaged => CallFunction(address, GenericsConverter.StructureToBytes(ref args));

        public bool CallFunction<T>(IntPtr address, T args) where T : unmanaged => CallFunction(address, GenericsConverter.StructureToBytes(args));
    }
}