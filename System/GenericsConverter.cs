using System.Runtime.InteropServices;

namespace System
{
    public static unsafe class GenericsConverter
    {
        public static byte[] StructureToBytes<T>(ref T structure) where T : unmanaged
        {
            int length = Marshal.SizeOf<T>();

            byte[] array = new byte[length];

            fixed (void* arrayPtr = array)
            {
                *(T*)arrayPtr = structure;
            }

            return array;
        }

        public static byte[] StructureToBytes<T>(T structure) where T : unmanaged => StructureToBytes(ref structure);

        public static T BytesToStructure<T>(byte[] bytes) where T : unmanaged
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (bytes.Length < Marshal.SizeOf<T>())
            {
                throw new ArgumentOutOfRangeException("bytes", "bytes length smaller than the size of the structure");
            }

            fixed (byte* bytesPtr = bytes)
            {
                return *(T*)bytesPtr;
            }
        }

        public static byte[] ManagedToBytes<T>(T managedType)
        {
            int size = Marshal.SizeOf<T>();
            byte[] bytes = new byte[size];

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            Marshal.StructureToPtr(managedType, handle.AddrOfPinnedObject(), true);

            handle.Free();

            return bytes;
        }

        public static T BytesToManaged<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            int size = Marshal.SizeOf<T>();

            if (bytes.Length < size)
            {
                throw new ArgumentOutOfRangeException("bytes", "bytes length smaller than the size of the structure");
            }

            IntPtr typePtr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 0, typePtr, size);

            T type = Marshal.PtrToStructure<T>(typePtr);

            Marshal.FreeHGlobal(typePtr);

            return type;
        }
    }
}