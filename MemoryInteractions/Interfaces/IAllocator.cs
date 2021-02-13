using System.WinApi;

namespace System.MemoryInteraction
{
    public interface IAllocator:IDisposable
    {
        IntPtr Alloc(int size);

        IntPtr Alloc(IntPtr size);

        IntPtr Reset(IntPtr address, int size);

        IntPtr Reset(IntPtr address, IntPtr size);

        IntPtr Undo(IntPtr address, int size);

        IntPtr Undo(IntPtr address, IntPtr size);

        bool Protect(IntPtr address, int size, AllocationProtect protectCode, out AllocationProtect oldProtect);

        bool Protect(IntPtr address, IntPtr size, AllocationProtect protectCode, out AllocationProtect oldProtect);

        bool Free(IntPtr address);
    }
}