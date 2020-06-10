using System;
using System.Diagnostics;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface ISimpleMemory:IDisposable
    {
        byte[] ReadBytes(IntPtr address, int size);

        byte[] ReadBytes(IntPtr address, IntPtr size);

        bool WriteBytes(IntPtr address, byte[] buffer);
    }
}