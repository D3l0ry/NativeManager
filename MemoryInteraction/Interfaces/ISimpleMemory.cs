using System;
using System.Diagnostics;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface ISimpleMemory
    {
        IntPtr Handle { get; }

        Process SelectedProcess { get; }

        byte[] ReadBytes(IntPtr address, int size);

        bool WriteBytes(IntPtr address, byte[] buffer);
    }
}