using System;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface ISimpleMemory
    {
        byte[] ReadBytes(IntPtr address, int size);

        bool WriteBytes(IntPtr address, byte[] buffer);
    }
}