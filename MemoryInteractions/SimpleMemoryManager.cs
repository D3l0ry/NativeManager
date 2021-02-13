using System.Collections.Generic;
using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public class SimpleMemoryManager : ISimpleMemory
    {
        protected Process m_Process;

        public SimpleMemoryManager(Process process)
        {
            if(process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            m_Process = process;
        }

        ~SimpleMemoryManager()
        {
            Dispose();
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

        public byte[] this[IntPtr address, Predicate<byte> predicate] => ReadBytes(address, predicate);

        public virtual byte[] ReadBytes(IntPtr address, IntPtr size)
        {
            byte[] buffer = new byte[size.ToInt32()];

            Kernel32.ReadProcessMemory(m_Process.Handle, address, buffer, size, IntPtr.Zero);

            return buffer;
        }

        public virtual byte[] ReadBytes(IntPtr address, int size) => ReadBytes(address, (IntPtr)size);

        public virtual byte[] ReadBytes(IntPtr address, Predicate<byte> predicate)
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

            return buffer.ToArray();
        }

        public virtual bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(m_Process.Handle, address, buffer, (IntPtr)buffer.Length, IntPtr.Zero);

        public void Dispose()
        {
            m_Process = null;

            GC.SuppressFinalize(this);
        }
    }
}