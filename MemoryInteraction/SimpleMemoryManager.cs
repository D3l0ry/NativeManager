using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public class SimpleMemoryManager : ISimpleMemory
    {
        private bool m_disposed;
        protected readonly Process m_Process;

        public SimpleMemoryManager(Process process) => m_Process = process;

        ~SimpleMemoryManager()
        {
            Dispose(false);
        }

        public byte[] this[IntPtr address, IntPtr size]
        {
            get => ReadBytes(address, size);
            set => WriteBytes(address, value);
        }

        public byte[] this[IntPtr address, int size]
        {
            get => ReadBytes(address, size);
            set => WriteBytes(address, value);
        }

        public virtual byte[] ReadBytes(IntPtr address, IntPtr size)
        {
            byte[] buffer = new byte[size.ToInt32()];

            Kernel32.ReadProcessMemory(m_Process.Handle, address, buffer, size, IntPtr.Zero);

            return buffer;
        }

        public virtual byte[] ReadBytes(IntPtr address, int size) => ReadBytes(address, (IntPtr)size);

        public virtual bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(m_Process.Handle, address, buffer, (IntPtr)buffer.Length, IntPtr.Zero);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }

            if (disposing)
            {
                m_Process.Dispose();
            }

            m_disposed = true;
        }
    }
}