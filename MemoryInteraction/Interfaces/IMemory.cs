using System;
using System.Diagnostics;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IMemory : ISimpleMemory
    {
        IntPtr Handle { get; }

        Process ProcessMemory { get;}

        T Read<T>(IntPtr address) where T : unmanaged;

        bool Write<T>(IntPtr address, T value) where T : unmanaged;

        IAllocator GetAllocator();
    }
}