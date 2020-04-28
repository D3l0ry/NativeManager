using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.ProcessInteraction;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public unsafe class MemoryManager : SimpleMemoryManager, IMemory, IDisposable
    {
        #region Private variables
        private Allocator m_Allocator;
        private Executor m_Executor;
        private PageManager m_PageManager;
        private PatternManager m_PatternManager;
        private ProcessInfo m_ProcessInfo;
        #endregion

        #region Public properties
        public Process ProcessMemory { get; private set; }
        #endregion

        public MemoryManager(Process process, ProcessAccess access = ProcessAccess.ALL) : base(process, access) => ProcessMemory = process;

        public MemoryManager(string processName, int index = 0, ProcessAccess access = ProcessAccess.ALL) : base(processName, index, access) => ProcessMemory = Process.GetProcessesByName(processName)[index];

        ~MemoryManager()
        {
            Kernel32.CloseHandle(Handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Read<T>(IntPtr address) where T : unmanaged => Executor.ByteToStructure<T>(ReadBytes(address, Marshal.SizeOf<T>()));

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
        public virtual bool Write<T>(IntPtr address, T value) where T : unmanaged => WriteBytes(address, Executor.StructureToByte(value));

        #region Operation with memory
        public virtual bool BlockCopy<TArray>(TArray[] src, int index, IntPtr dst, int dstOffset, int count) where TArray : unmanaged
        {
            if (count == 0)
            {
                return false;
            }

            if (index >= src.Length)
            {
                throw new IndexOutOfRangeException("index is more than the length of the array");
            }

            if (count > src.Length - index)
            {
                throw new IndexOutOfRangeException("count is more than the length of the array");
            }

            fixed (TArray* srcPtr = &src[index])
            {
                return Kernel32.WriteProcessMemory(Handle, dst + dstOffset, (IntPtr)srcPtr, count * sizeof(TArray), IntPtr.Zero);
            }
        }

        public virtual bool MemoryCopy(IntPtr src, int srcOffset, IntPtr dst, int dstOffset, int count)
        {
            if (count == 0)
            {
                return false;
            }

            return Kernel32.WriteProcessMemory(Handle, dst + dstOffset, src + srcOffset, count, IntPtr.Zero);
        }
        #endregion

        #region Operations with allocator
        public IAllocator GetAllocator()
        {
            if (m_Allocator == null)
            {
                m_Allocator = new Allocator(this);
            }

            return m_Allocator;
        }
        #endregion

        #region Operations with executor
        public Executor GetExecutor()
        {
            if (m_Executor == null)
            {
                m_Executor = new Executor(this);
            }

            return m_Executor;
        }
        #endregion

        #region Operations with page
        public PageManager GetPageManager()
        {
            if (m_PageManager == null)
            {
                m_PageManager = new PageManager(this);
            }

            return m_PageManager;
        }
        #endregion

        #region Operation with pattern
        public PatternManager GetPatternManager()
        {
            if (m_PatternManager == null)
            {
                m_PatternManager = new PatternManager(this);
            }

            return m_PatternManager;
        }
        #endregion

        #region Operations with process
        public ProcessInfo GetProcessInfo()
        {
            if (m_ProcessInfo == null)
            {
                m_ProcessInfo = new ProcessInfo(ProcessMemory);
            }

            return m_ProcessInfo;
        }
        #endregion

        public void Dispose() => Kernel32.CloseHandle(Handle);
    }
}