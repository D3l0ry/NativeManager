using NativeManager.ProcessManager;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        public virtual T Read<T>(int address) => Read<T>((long)address);

        public virtual T Read<T>(uint address) => Read<T>((long)address);

        public virtual T Read<T>(long address)
        {
            fixed (byte* buffer = ReadBytes((void*)address, Marshal.SizeOf<T>()))
            {
                return Marshal.PtrToStructure<T>((IntPtr)buffer);
            }
        }

        public virtual bool Write<T>(int address, T value)
        {
            byte[] buffer = new byte[Marshal.SizeOf<T>()];

            fixed (byte* bufferPtr = buffer)
            {
                Marshal.StructureToPtr(value, (IntPtr)bufferPtr, true);
            }

            return WriteBytes((void*)address, buffer);
        }

        public IntPtr Allocate(uint size) => Kernel32.VirtualAllocEx(Handle, IntPtr.Zero, size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);

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
            IntPtr alloc = Allocate((uint)args.Length);

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

        public IntPtr FindPattern(uint module, string pattern, int offset = 0) => Pattern.FindPattern(this, module, pattern, offset);

        public void Dispose() => Kernel32.CloseHandle(Handle);
    }
}