using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.ProcessInteraction;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public class SimpleMemoryManager : ISimpleMemory
    {
        protected readonly IntPtr m_Handle;

        public Process SelectedProcess { get; private set; }

        public SimpleMemoryManager(Process process)
        {
            SelectedProcess = process;
            m_Handle = process.Handle;
        }

        public SimpleMemoryManager(string processName, int index = 0) : this(ProcessInfo.Exists(Process.GetProcessesByName(processName), index)) { }

        public SimpleMemoryManager(int processId) : this(Process.GetProcessById(processId)) { }

        ~SimpleMemoryManager()
        {
            Dispose();
        }

        public virtual byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];

            if (Kernel32.ReadProcessMemory(m_Handle, address, buffer, size, IntPtr.Zero))
            {
                return buffer;
            }

            return buffer;
        }

        public virtual bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(m_Handle, address, buffer, buffer.Length, IntPtr.Zero);

        public void Dispose() => Kernel32.CloseHandle(m_Handle);
    }
}