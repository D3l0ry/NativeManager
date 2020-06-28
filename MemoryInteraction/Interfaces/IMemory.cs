namespace System.MemoryInteraction
{
    public interface IMemory:ISimpleMemory
    {
        T Read<T>(IntPtr address) where T : unmanaged;

        bool Write<T>(IntPtr address, T value) where T : unmanaged;

        IAllocator GetAllocator();
    }
}