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

        public static byte[] StructureToByte<T>(T structure) where T : unmanaged
        {
            int length = Marshal.SizeOf<T>();

            byte[] array = new byte[length];

            fixed (void* arrayPtr = array)
            {
                Buffer.MemoryCopy(&structure, arrayPtr, length, length);
            }

            return array;
        }

        public static T ByteToStructure<T>(byte[] bytes) where T : unmanaged
        {
            if(bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if(bytes.Length < Marshal.SizeOf<T>())
            {
                throw new ArgumentOutOfRangeException("bytes", "bytes length smaller than the size of the structure");
            }

            fixed (byte* bytesPtr = bytes)
            {
                return *(T*)bytesPtr;
            }
        }

        public bool CallFunction(IntPtr address, byte[] args)
        {
            IntPtr alloc = IntPtr.Zero;
            bool execute = false;

            try
            {
                alloc = m_Memory.GetAllocator().Alloc((uint)args.Length);

                if (alloc == IntPtr.Zero)
                {
                    return false;
                }

                if (!m_Memory.WriteBytes(alloc, args))
                {
                    return false;
                }

                execute = Execute(address, alloc);
            }
            finally
            {
                m_Memory.GetAllocator().Free(alloc);
            }

            return execute;
        }

        public bool CallFunction<T>(IntPtr address, T args) where T : unmanaged => CallFunction(address, StructureToByte(args));
    }
}