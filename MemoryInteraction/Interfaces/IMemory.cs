using System;

namespace NativeManager.MemoryInteraction.Interfaces
{
    public interface IMemory:ISimpleMemory,IAllocator
    {
        IntPtr Handle { get; }

        T Read<T>(IntPtr address);

        bool Write<T>(IntPtr address, T value);
    }
}