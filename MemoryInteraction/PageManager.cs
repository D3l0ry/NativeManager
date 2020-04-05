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
        #region Private variables
        private readonly IMemory m_Memory;
        #endregion

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
            List<MEMORY_BASIC_INFORMATION> pages = new List<MEMORY_BASIC_INFORMATION>(10);
            SYSTEM_INFO systemInfo = GetSystemInfo();

            byte* startAddress = (byte*)address.ToPointer();
            byte* maximumAddress = (byte*)systemInfo.lpMaximumApplicationAddress.ToPointer();

            while(startAddress < maximumAddress)
            {
                pages.Add(GetPage((IntPtr)startAddress));

                startAddress += systemInfo.dwPageSize;
            }

            return pages.ToArray();
        }

        private SYSTEM_INFO GetSystemInfo()
        {
            SYSTEM_INFO systemInfo = new SYSTEM_INFO();

            Kernel32.GetSystemInfo(ref systemInfo);

            return systemInfo;
        }

        private MEMORY_BASIC_INFORMATION GetPage(IntPtr address)
        {
            if (!VirtualQuery(address, out MEMORY_BASIC_INFORMATION pageInformation))
            {
                throw new Win32Exception("VirtualQuery returned zero");
            }

            return pageInformation;
        }

        private bool VirtualQuery(IntPtr address, out MEMORY_BASIC_INFORMATION pageInformation)
        {
            if (Kernel32.VirtualQueryEx(m_Memory.Handle, address, out pageInformation, (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()) == 0)
            {
                return false;
            }

            return true;
        }
    }
}