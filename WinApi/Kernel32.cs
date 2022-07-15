using System.Runtime.InteropServices;
using System.Security;

namespace System.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class Kernel32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, FreeType dwFreeType);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationProtect flNewProtect, out AllocationProtect lpflOldProtect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, uint dwSize, IntPtr lpNumberOfBytesRead);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] byte[] lpBuffer, uint nSize, IntPtr lpNumberOfBytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, IntPtr dwCreationFlags, IntPtr lpThreadId);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ConstLibrariesName.KernelDll)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport(ConstLibrariesName.KernelDll, SetLastError = true)]
        public static extern void GetSystemInfo(ref SYSTEM_INFO Info);

        [DllImport(ConstLibrariesName.KernelDll, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport(ConstLibrariesName.KernelDll, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}