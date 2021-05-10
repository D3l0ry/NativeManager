namespace System.MemoryInteraction
{
    public interface IMemory
    {
        T Read<T>(IntPtr address);

        bool Write<T>(IntPtr address, T value);
    }
}