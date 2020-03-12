using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NativeManager.ProcessManager;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.VirtualMemory
{
    public unsafe class HMemory : IDisposable
    {
        #region Variables
        public Process ProcessMemory { get; private set; }

        public IntPtr Handle { get; private set; }
        #endregion

        #region Initialization
        public HMemory(string processName, int index = 0, ProcessAccess access = ProcessAccess.All)
        {
            Process[] LocalProc = Process.GetProcessesByName(processName);

            ProcessInfo.Found(LocalProc, index);

            ProcessMemory = LocalProc[index];
            Handle = Kernel32.OpenProcess(access, false, ProcessMemory.Id);

            if (Handle == IntPtr.Zero)
            {
                throw new NullReferenceException("Failed to open process descriptor");
            }
        }

        public HMemory(Process process, ProcessAccess access = ProcessAccess.All)
        {
            ProcessMemory = process;
            Handle = Kernel32.OpenProcess(access, false, ProcessMemory.Id);

            if (Handle == IntPtr.Zero)
            {
                throw new NullReferenceException("Failed to open process descriptor");
            }
        }
        #endregion

        private byte[] ReadBytes(void* address, int size)
        {
            byte[] buffer = new byte[size];

            if (Kernel32.ReadProcessMemory(Handle, address, buffer, size, IntPtr.Zero))
            {
                return buffer;
            }

            return new byte[1];
        }

        private bool WriteBytes(void* address, byte[] buffer) => Kernel32.WriteProcessMemory(Handle, address, buffer, buffer.Length, IntPtr.Zero);

#if WIN32
        public T Read<T>(int address) => Read<T>((uint)address);

        public T Read<T>(uint address)
        {
            fixed (byte* buffer = ReadBytes((void*)address, Marshal.SizeOf<T>()))
            {
                return Marshal.PtrToStructure<T>((IntPtr)buffer);
            }
        }

        public T[] Read<T>(int address, int size) => Read<T>((uint)address, size);

        public T[] Read<T>(uint address, int size)
        {
            T[] elements = new T[size];

            for (int index = 0; index < size; index++)
            {
                elements[index] = Read<T>((uint)(address + (index * size)));
            }

            return elements;
        }

#elif WIN64
        public virtual T Read<T>(ulong address)
        {
            fixed (byte* buffer = ReadBytes((void*)address, Marshal.SizeOf<T>()))
            {
                return Marshal.PtrToStructure<T>((IntPtr)buffer);
            }
        }
#endif

#if WIN32
        public virtual bool Write<T>(int address, T value) => Write((uint)address, value);

        public virtual bool Write<T>(uint address, T value)
        {
            byte[] buffer = new byte[Marshal.SizeOf<T>()];

            fixed (byte* bufferPtr = buffer)
            {
                Marshal.StructureToPtr(value, (IntPtr)bufferPtr, true);
            }

            return WriteBytes((void*)address, buffer);
        }

#elif WIN64
        public virtual bool Write<T>(ulong address, T value)
        {
            byte[] buffer = new byte[Marshal.SizeOf<T>()];

            fixed (byte* bufferPtr = buffer)
            {
                Marshal.StructureToPtr(value, (IntPtr)bufferPtr, true);
            }

            return WriteBytes((void*)address, buffer);
        }
#endif

        #region Working with modules
        public ProcessModule GetModule(string module) => ProcessMemory.GetModule(module);

        public Dictionary<string, ProcessModule> GetModules() => ProcessMemory.GetModules();

        public Dictionary<string, IntPtr> GetAddressModules() => ProcessMemory.GetAddressModules();
        #endregion

        #region Memory operation
        public IntPtr Alloc(uint size) => Kernel32.VirtualAllocEx(Handle, IntPtr.Zero, size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);

        public bool Protect(IntPtr address, uint size, ProtectCode protectCode, out ProtectCode oldProtect) => Kernel32.VirtualProtectEx(Handle, address, (UIntPtr)size, protectCode, out oldProtect);

        public bool Execute(IntPtr address, IntPtr args)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(Handle, IntPtr.Zero, 0, address, args, 0, IntPtr.Zero);

            Kernel32.WaitForSingleObject(thread, 0xFFFFFFFF);
            Kernel32.CloseHandle(thread);

            return thread != IntPtr.Zero;
        }

        public bool Free(IntPtr address) => Kernel32.VirtualFreeEx(Handle, address, 0, AllocationType.Release);

        public byte[] StructureToByte<T>(T structure)
        {
            int length = Marshal.SizeOf<T>();

            byte[] array = new byte[length];

            IntPtr ptr = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(structure, ptr, true);

            Marshal.Copy(ptr, array, 0, length);

            Marshal.FreeHGlobal(ptr);

            return array;
        }

        public bool CallFunction(IntPtr address, byte[] args)
        {
            IntPtr alloc = Alloc((uint)args.Length);

            if (alloc == IntPtr.Zero)
            {
                return false;
            }

            if (!WriteBytes(alloc.ToPointer(), args))
            {
                return false;
            }

            bool execute = Execute(address, alloc);

            Free(alloc);

            return execute;
        }

        public bool CallFunction<T>(IntPtr address, T args) => CallFunction(address, StructureToByte(args));

        public bool BlockCopy<TArray>(TArray[] src, int srcOffset, void* dst, int dstOffset, int count) where TArray : unmanaged
        {
            if (srcOffset > src.Length - 1)
            {
                throw new IndexOutOfRangeException("srcOffset is more than the length of the array");
            }

            fixed (TArray* srcPtr = src)
            {
                return Kernel32.WriteProcessMemory(Handle, (byte*)dst + dstOffset, srcPtr + srcOffset, count * sizeof(TArray), IntPtr.Zero);
            }
        }

        public bool MemoryCopy<TSrc>(TSrc* src, int srcOffset, void* dst, int dstOffset, int count) where TSrc : unmanaged => Kernel32.WriteProcessMemory(Handle, (byte*)dst + dstOffset, (byte*)src + srcOffset, count, IntPtr.Zero);
        #endregion

        public IntPtr FindPattern(uint module, byte[] pattern, int offset = 0) => Pattern.FindPattern(this, module, pattern, offset);

        public IntPtr FindPattern(uint module, string pattern, int offset = 0) => Pattern.FindPattern(this, module, pattern, offset);

        public void Dispose() => Kernel32.CloseHandle(Handle);
    }
}