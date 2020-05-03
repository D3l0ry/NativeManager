using System;
using System.Diagnostics;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.ProcessInteraction;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public class SimpleMemoryManager:ISimpleMemory
    {
        public IntPtr Handle { get; private set; }

        public Process SelectedProcess { get; private set; }

        public SimpleMemoryManager(Process process, ProcessAccess access = ProcessAccess.ALL)
        {
            Handle = Kernel32.OpenProcess(access, false, process.Id);

            if (Handle == IntPtr.Zero)
            {
                throw new NullReferenceException("Failed to open process descriptor");
            }

            SelectedProcess = process;
        }

        public SimpleMemoryManager(string processName, int index = 0, ProcessAccess access = ProcessAccess.ALL):this(ProcessInfo.Exists(Process.GetProcessesByName(processName), index), access) { }

        public virtual byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];

            if (Kernel32.ReadProcessMemory(Handle, address, buffer, size, IntPtr.Zero))
            {
                return buffer;
            }

            return buffer;
        }

        public virtual bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(Handle, address, buffer, buffer.Length, IntPtr.Zero);
    }
}