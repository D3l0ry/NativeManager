using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.WinApi;

namespace System.MemoryInteractions
{
    public unsafe class PageManager
    {
        private readonly Process m_Process;

        public PageManager(Process process)
        {
            ProcessExtensions.CheckProcess(process);

            m_Process = process;
        }

        public MemoryBasicInformation this[IntPtr address] => GetPageInformation(address);

        private static MemoryBasicInformation GetPage(Process process, IntPtr address)
        {
            if (!VirtualQuery(process, address, out MemoryBasicInformation pageInformation))
            {
                throw process.ShowException<Win32Exception>(address, $"Не удалось получить информацию страницы по адресу: {address}");
            }

            return pageInformation;
        }

        private MemoryBasicInformation[] GetPages(IntPtr startAddress, IntPtr endAddress)
        {
            List<MemoryBasicInformation> pages = new List<MemoryBasicInformation>(5);

            long minAddress = startAddress.ToInt64();
            long maxAddress = endAddress.ToInt64();

            while (minAddress < maxAddress)
            {
                MemoryBasicInformation page = GetPage(m_Process, startAddress);

                pages.Add(page);

                minAddress += page.RegionSize.ToInt64();
            }

            return pages.ToArray();
        }

        private static SystemInfo GetSystemInfo()
        {
            SystemInfo systemInfo = new SystemInfo();

            Kernel32.GetSystemInfo(ref systemInfo);

            return systemInfo;
        }

        private static bool VirtualQuery(Process process, IntPtr address, out MemoryBasicInformation pageInformation)
        {
            uint memoryBasicInformationSize = (uint)Marshal.SizeOf<MemoryBasicInformation>();

            return Kernel32.VirtualQueryEx(process.Handle, address, out pageInformation, memoryBasicInformationSize) != 0;
        }

        public MemoryBasicInformation GetPageInformation(IntPtr address)
        {
            SystemInfo systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException($"Адрес {address} находится за пределами адресной области процесса");
            }

            return GetPage(m_Process, address);
        }

        public MemoryBasicInformation[] GetPagesInformation(IntPtr address)
        {
            SystemInfo systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException($"Адрес {address} находится за пределами адресной области процесса");
            }

            return GetPages(address, systemInfo.lpMaximumApplicationAddress);
        }

        public MemoryBasicInformation[] GetPagesInformation()
        {
            SystemInfo systemInfo = GetSystemInfo();

            return GetPages(systemInfo.lpMinimumApplicationAddress, systemInfo.lpMaximumApplicationAddress);
        }

        public static MemoryBasicInformation GetPageInformation(Process process, IntPtr address)
        {
            SystemInfo systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException($"Адрес {address} находится за пределами адресной области процесса");
            }

            return GetPage(process, address);
        }
    }
}