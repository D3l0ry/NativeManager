namespace System.MemoryInteraction
{
    public interface IMemory:ISimpleMemory
    {
        T Read<T>(IntPtr address);

        bool Write<T>(IntPtr address, T value);
    }
}