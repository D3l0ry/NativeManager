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
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

            m_Process = process;
        }

        public MEMORY_BASIC_INFORMATION this[IntPtr address] => GetPageInformation(address);

        private static MEMORY_BASIC_INFORMATION GetPage(Process process, IntPtr address)
        {
            if (!VirtualQuery(process, address, out MEMORY_BASIC_INFORMATION pageInformation))
            {
                throw process.ShowException<Win32Exception>(address, $"Не удалось получить информацию страницы по адресу: {address}");
            }

            return pageInformation;
        }

        private MEMORY_BASIC_INFORMATION[] GetPages(IntPtr startAddress, IntPtr endAddress)
        {
            List<MEMORY_BASIC_INFORMATION> pages = new List<MEMORY_BASIC_INFORMATION>(5);

            long minAddress = startAddress.ToInt64();
            long maxAddress = endAddress.ToInt64();

            while (minAddress < maxAddress)
            {
                MEMORY_BASIC_INFORMATION page = GetPage(m_Process, startAddress);

                pages.Add(page);

                minAddress += page.RegionSize.ToInt64();
            }

            return pages.ToArray();
        }

        private static SYSTEM_INFO GetSystemInfo()
        {
            SYSTEM_INFO systemInfo = new SYSTEM_INFO();

            Kernel32.GetSystemInfo(ref systemInfo);

            return systemInfo;
        }

        private static bool VirtualQuery(Process process, IntPtr address, out MEMORY_BASIC_INFORMATION pageInformation)
        {
            uint memoryBasicInformationSize = (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>();

            return Kernel32.VirtualQueryEx(process.Handle, address, out pageInformation, memoryBasicInformationSize) != 0;
        }

        public MEMORY_BASIC_INFORMATION GetPageInformation(IntPtr address)
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException($"Адрес {address} находится за пределами адресной области процесса");
            }

            return GetPage(m_Process, address);
        }

        public MEMORY_BASIC_INFORMATION[] GetPagesInformation(IntPtr address)
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException($"Адрес {address} находится за пределами адресной области процесса");
            }

            return GetPages(address, systemInfo.lpMaximumApplicationAddress);
        }

        public MEMORY_BASIC_INFORMATION[] GetPagesInformation()
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            return GetPages(systemInfo.lpMinimumApplicationAddress, systemInfo.lpMaximumApplicationAddress);
        }

        public static MEMORY_BASIC_INFORMATION GetPageInformation(Process process, IntPtr address)
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException($"Адрес {address} находится за пределами адресной области процесса");
            }

            return GetPage(process, address);
        }
    }
}