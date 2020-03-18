using System;
using System.Runtime.InteropServices;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.WinApi;

namespace NativeManager.MemoryInteraction
{
    public unsafe class Executor
    {
        #region Private variables
        private readonly IMemory m_Memory;
        #endregion

        public Executor(IMemory memory) => m_Memory = memory;

        public bool Execute(IntPtr address, IntPtr args)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(m_Memory.Handle, IntPtr.Zero, 0, address, args, 0, IntPtr.Zero);

            Kernel32.WaitForSingleObject(thread, 0xFFFFFFFF);
            Kernel32.CloseHandle(thread);

            return thread != IntPtr.Zero;
        }

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

        public T ByteToStructure<T>(byte[] bytes)
        {
            fixed (byte* bytesPtr = bytes)
            {
                return Marshal.PtrToStructure<T>((IntPtr)bytesPtr);
            }
        }

        public bool CallFunction(IntPtr address, byte[] args)
        {
            IntPtr alloc = IntPtr.Zero;
            bool execute = false;

            try
            {
                alloc = m_Memory.Alloc((uint)args.Length);

                if (alloc == IntPtr.Zero)
                {
                    return false;
                }

                if (!m_Memory.WriteBytes(alloc,args))
                {
                    return false;
                }

                execute = Execute(address, alloc);
            }
            finally
            {
                m_Memory.Free(alloc);
            }

            return execute;
        }

        public bool CallFunction<T>(IntPtr address, T args) => CallFunction(address, StructureToByte(args));
    }
}