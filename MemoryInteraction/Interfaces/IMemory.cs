using System;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IMemory : ISimpleMemory
    {
        IntPtr Handle { get; }

        T Read<T>(IntPtr address) where T : unmanaged;

        bool Write<T>(IntPtr address, T value) where T : unmanaged;

        IAllocator GetAllocator();
    }
}