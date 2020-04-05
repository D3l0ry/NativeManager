using System;
using System.Runtime.InteropServices;
using System.Security;

using NativeManager.WinApi.Enums;

namespace NativeManager.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class Kernel32
    {
        private const string m_KernelDll = "kernel32.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccess processAccess, bool bInheritHandle, int processId);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, AllocationProtect flNewProtect, out AllocationProtect lpflOldProtect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern void GetSystemInfo(ref SYSTEM_INFO Info);
    }
}