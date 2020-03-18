using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.ProcessInteraction;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public unsafe class MemoryManager : IMemory, IAllocator, IDisposable
    {
        #region Private variables
        private Executor m_Executor;
        private PatternManager m_PatternManager;
        private ProcessInfo m_ProcessInfo;
        #endregion

        #region Public properties
        public IntPtr Handle { get; private set; }

        public Process ProcessMemory { get; private set; }
        #endregion

        public MemoryManager(Process process, ProcessAccess access = ProcessAccess.All)
        {
            ProcessMemory = process;
            Handle = Kernel32.OpenProcess(access, false, ProcessMemory.Id);

            if (Handle == IntPtr.Zero)
            {
                throw new NullReferenceException("Failed to open process descriptor");
            }
        }

        public MemoryManager(string processName, int index = 0, ProcessAccess access = ProcessAccess.All)
        {
            Process[] localProcess = Process.GetProcessesByName(processName);

            ProcessInfo.Exists(localProcess, index);

            ProcessMemory = localProcess[index];
            Handle = Kernel32.OpenProcess(access, false, ProcessMemory.Id);

            if (Handle == IntPtr.Zero)
            {
                throw new NullReferenceException("Failed to open process descriptor");
            }
        }

        ~MemoryManager()
        {
            Kernel32.CloseHandle(Handle);
        }

        public byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];

            if (Kernel32.ReadProcessMemory(Handle, address, buffer, size, IntPtr.Zero))
            {
                return buffer;
            }

            return new byte[1];
        }

        public bool WriteBytes(IntPtr address, byte[] buffer) => Kernel32.WriteProcessMemory(Handle, address, buffer, buffer.Length, IntPtr.Zero);

        public T Read<T>(IntPtr address)
        {
            fixed (byte* buffer = ReadBytes(address, Marshal.SizeOf<T>()))
            {
                return Marshal.PtrToStructure<T>((IntPtr)buffer);
            }
        }

        public T[] Read<T>(IntPtr address, int count)
        {
            int size = Marshal.SizeOf<T>();

            T[] elements = new T[count];

            for (int index = 0; index < count; index++)
            {
                elements[index] = Read<T>(address + (index * size));
            }

            return elements;
        }

        public bool Write<T>(IntPtr address, T value)
        {
            byte[] buffer = new byte[Marshal.SizeOf<T>()];

            fixed (byte* bufferPtr = buffer)
            {
                Marshal.StructureToPtr(value, (IntPtr)bufferPtr, true);
            }

            return WriteBytes(address, buffer);
        }

        #region Operation with memory
        public IntPtr Alloc(uint size) => Kernel32.VirtualAllocEx(Handle, IntPtr.Zero, size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);

        public bool Protect(IntPtr address, uint size, ProtectCode protectCode, out ProtectCode oldProtect) => Kernel32.VirtualProtectEx(Handle, address, (UIntPtr)size, protectCode, out oldProtect);

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(Handle, address, 0, AllocationType.Release);

        public bool BlockCopy<TArray>(TArray[] src, int srcOffset, IntPtr dst, int dstOffset, int count) where TArray : unmanaged
        {
            if (srcOffset > src.Length - 1)
            {
                throw new IndexOutOfRangeException("srcOffset is more than the length of the array");
            }

            fixed (TArray* srcPtr = src)
            {
                return Kernel32.WriteProcessMemory(Handle, dst + dstOffset, (IntPtr)(srcPtr + srcOffset), count * sizeof(TArray), IntPtr.Zero);
            }
        }

        public bool MemoryCopy(IntPtr src, int srcOffset, IntPtr dst, int dstOffset, int count) => Kernel32.WriteProcessMemory(Handle, dst + dstOffset, src + srcOffset, count, IntPtr.Zero);
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