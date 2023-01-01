using System.Runtime.InteropServices;

namespace System
{
    public static unsafe class GenericsConverter
    {
        /// <summary>
        /// Преобразует массив байт в неуправляемый тип
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structure">Неуправляемый объект</param>
        /// <returns></returns>
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

        /// <summary>
        /// Преобразует массив байт в неуправляемый тип
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structure">Неуправляемый объект</param>
        /// <returns></returns>
        public static byte[] StructureToBytes<T>(T structure) where T : unmanaged => StructureToBytes(ref structure);

        /// <summary>
        /// Преобразует массив байт в неуправляемый тип
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes">Массив байт</param>
        /// <exception cref=" ArgumentNullException"></exception>
        /// <exception cref=" ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static T BytesToStructure<T>(byte[] bytes) where T : unmanaged
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            int size = Marshal.SizeOf<T>();

            if (bytes.Length < size)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Размер массива меньше размера структуры");
            }

            fixed (byte* arrayPtr = bytes)
            {
                return *(T*)arrayPtr;
            }
        }

        /// <summary>
        /// Преобразует управляемый тип в массив байт
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="managedType">Управляемый объект</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static byte[] ManagedToBytes<T>(T managedType)
        {
            if (managedType == null)
            {
                throw new ArgumentNullException(nameof(managedType));
            }

            int size = Marshal.SizeOf<T>();
            byte[] bytes = new byte[size];

            fixed (byte* bytePtr = bytes)
            {
                Marshal.StructureToPtr(managedType, (IntPtr)bytePtr, true);
            }

            return bytes;
        }

        /// <summary>
        /// Преобразует массив байт в управляемый тип (С неуправляемыми переменными)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes">Массив байт</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <returns></returns>
        public static T BytesToManaged<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            int size = Marshal.SizeOf<T>();

            if (bytes.Length < size)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Размер массива меньше размера объекта");
            }

            fixed (byte* bytePtr = bytes)
            {
                return Marshal.PtrToStructure<T>((IntPtr)bytePtr);
            }
        }
    }
}