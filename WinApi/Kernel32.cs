﻿using System.Runtime.InteropServices;
using System.Security;

namespace System.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class Kernel32
    {
        private const string m_KernelDll = "kernel32.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, FreeType dwFreeType);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, AllocationProtect flNewProtect, out AllocationProtect lpflOldProtect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, IntPtr nSize, IntPtr lpNumberOfBytesRead);

        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out, MarshalAs(UnmanagedType.AsAny)] out object lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] byte[] lpBuffer, IntPtr nSize, IntPtr lpNumberOfBytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] IntPtr lpBuffer, IntPtr nSize, IntPtr lpNumberOfBytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In][MarshalAs(UnmanagedType.AsAny)] object lpBuffer, IntPtr nSize, IntPtr lpNumberOfBytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, IntPtr dwCreationFlags, IntPtr lpThreadId);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(m_KernelDll)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, IntPtr dwLength);

        [DllImport(m_KernelDll, SetLastError = true)]
        public static extern void GetSystemInfo(ref SYSTEM_INFO Info);

        [DllImport(m_KernelDll, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport(m_KernelDll, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}