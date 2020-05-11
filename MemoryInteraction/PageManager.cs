using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.MemoryInteraction
{
    public unsafe class PageManager
    {
        private readonly IMemory m_Memory;

        public PageManager(IMemory memory) => m_Memory = memory;

        public MEMORY_BASIC_INFORMATION this[IntPtr address]
        {
            get
            {
                return GetPageInformation(address);
            }
        }

        public MEMORY_BASIC_INFORMATION GetPageInformation(IntPtr address)
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            if (address.ToPointer()  > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException("The address is greater than the maximum application address");
            }

            return GetPage(address);
        }

        public MEMORY_BASIC_INFORMATION[] GetPagesInformation(IntPtr address)
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            if (address.ToPointer() > systemInfo.lpMaximumApplicationAddress.ToPointer())
            {
                throw new ArgumentOutOfRangeException("The address is greater than the maximum application address");
            }

            return GetPages(address, systemInfo.lpMaximumApplicationAddress);
        }

        public MEMORY_BASIC_INFORMATION[] GetPagesInformation()
        {
            SYSTEM_INFO systemInfo = GetSystemInfo();

            return GetPages(systemInfo.lpMinimumApplicationAddress, systemInfo.lpMaximumApplicationAddress);
        }

        private MEMORY_BASIC_INFORMATION GetPage(IntPtr address)
        {
            if (!VirtualQuery(address, out MEMORY_BASIC_INFORMATION pageInformation))
            {
                throw new Win32Exception("VirtualQuery returned zero");
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
                MEMORY_BASIC_INFORMATION page = GetPage(startAddress);

                pages.Add(page);

                minAddress += page.RegionSize.ToInt64();
            }

            return pages.ToArray();
        }

        private SYSTEM_INFO GetSystemInfo()
        {
            SYSTEM_INFO systemInfo = new SYSTEM_INFO();

            Kernel32.GetSystemInfo(ref systemInfo);

            return systemInfo;
        }

        private bool VirtualQuery(IntPtr address, out MEMORY_BASIC_INFORMATION pageInformation) => Kernel32.VirtualQueryEx(m_Memory.SelectedProcess.Handle, address, out pageInformation, (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()) != 0 ? true : false;
    }
}