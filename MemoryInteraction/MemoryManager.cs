using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe class MemoryManager : SimpleMemoryManager, IMemory
    {
        #region Private variables
        private Allocator m_Allocator;
        private Executor m_Executor;
        private PageManager m_PageManager;
        private PatternManager m_PatternManager;
        #endregion

        public MemoryManager(Process process) : base(process) { }

        #region Read and Write
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address) where T : unmanaged => GenericsConverter.BytesToStructure<T>(ReadBytes(address, Marshal.SizeOf<T>()));

        public virtual T[] Read<T>(IntPtr address, int count) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();

            T[] elements = new T[count];

            for (int index = 0; index < count; index++)
            {
                elements[index] = Read<T>(address + (index * size));
            }

            return elements;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Write<T>(IntPtr address, T value) where T : unmanaged => WriteBytes(address, GenericsConverter.StructureToBytes(value));
        #endregion

        #region Operation with memory
        public virtual bool BlockCopy<TArray>(TArray[] src, int srcIndex, IntPtr dst, int dstOffset, IntPtr count) where TArray : unmanaged
        {
            if (count == IntPtr.Zero)
            {
                return false;
            }

            if (srcIndex >= src.Length)
            {
                throw new IndexOutOfRangeException("index is more than the length of the array");
            }

            if (count.ToInt64() > src.Length - srcIndex)
            {
                throw new IndexOutOfRangeException("count is more than the length of the array");
            }

            fixed (TArray* srcPtr = &src[srcIndex])
            {
                return Kernel32.WriteProcessMemory(m_Process.Handle, dst + dstOffset, (IntPtr)srcPtr, (IntPtr)(count.ToInt64() * sizeof(TArray)), IntPtr.Zero);
            }
        }

        public virtual bool MemoryCopy(IntPtr src, int srcOffset, IntPtr dst, int dstOffset, IntPtr count)
        {
            if (count == IntPtr.Zero)
            {
                return false;
            }

            return Kernel32.WriteProcessMemory(m_Process.Handle, dst + dstOffset, src + srcOffset, count, IntPtr.Zero);
        }
        #endregion

        #region Operations with allocator
        public IAllocator GetAllocator()
        {
            if (m_Allocator == null)
            {
                m_Allocator = new Allocator(m_Process);
            }

            return m_Allocator;
        }
        #endregion

        #region Operations with executor
        public Executor GetExecutor()
        {
            if (m_Executor == null)
            {
                m_Executor = new Executor(m_Process, this);
            }

            return m_Executor;
        }
        #endregion

        #region Operations with page
        public PageManager GetPageManager()
        {
            if (m_PageManager == null)
            {
                m_PageManager = new PageManager(m_Process);
            }

            return m_PageManager;
        }
        #endregion

        #region Operation with pattern
        public PatternManager GetPatternManager()
        {
            if (m_PatternManager == null)
            {
                m_PatternManager = new PatternManager(m_Process,this);
            }

            return m_PatternManager;
        }
        #endregion

        #region Static methods
        public static MemoryManager GetMemoryCurrentProcess() => new MemoryManager(Process.GetCurrentProcess());
        #endregion
    }
}