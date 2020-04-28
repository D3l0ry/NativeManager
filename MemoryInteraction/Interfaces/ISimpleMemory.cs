using System;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface ISimpleMemory
    {
        IntPtr Handle { get; }

        byte[] ReadBytes(IntPtr address, int size);

        bool WriteBytes(IntPtr address, byte[] buffer);
    }
}