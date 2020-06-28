using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public class SimpleMemoryManager : ISimpleMemory
    {
        protected readonly Process m_Process;

        public SimpleMemoryManager(Process process) => m_Process = process;

        ~SimpleMemoryManager()
        {
            Dispose();
        }

        public virtual byte[] ReadBytes(IntPtr address, int size) => ReadBytes(address, (IntPtr)size);

        public virtual byte[] ReadBytes(IntPtr address, IntPtr size)
        {
            byte[] buffer = new byte[size.ToInt32()];

            Kernel32.ReadProcessMemory(m_Process.Handle, address, buffer, size, IntPtr.Zero);

            return buffer;
        }

        public virtual bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(m_Process.Handle, address, buffer, (IntPtr)buffer.Length, IntPtr.Zero);

        public void Dispose() => Kernel32.CloseHandle(m_Process.Handle);
    }
}