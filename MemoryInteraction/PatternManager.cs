using System;
using System.Collections.Generic;
using System.Linq;

using NativeManager.WinApi.Enums;
using NativeManager.WinApi;
using NativeManager.MemoryInteraction.Interfaces;

namespace NativeManager.MemoryInteraction
{
    public unsafe class PatternManager
    {
        #region Private variables
        private readonly IMemory m_MemoryManager;
        #endregion

        public PatternManager(IMemory memory) => m_MemoryManager = memory;

        public IntPtr FindPattern(IntPtr module, byte[] pattern, int offset = 0)
        {
            if (pattern == null)
            {
                return IntPtr.Zero;
            }

            Kernel32.GetModuleInformation(m_MemoryManager.Handle, module, out MODULEINFO MODULEINFO, sizeof(MODULEINFO));

            for (long PIndex = module.ToInt64(); PIndex < MODULEINFO.SizeOfImage; PIndex++)
            {
                bool Found = true;

                for (int MIndex = 0; MIndex < pattern.Length; MIndex++)
                {
                    Found = pattern[MIndex] == 0 || m_MemoryManager.Read<byte>((IntPtr)(PIndex + MIndex)) == pattern[MIndex];

                    if (!Found)
                    {
                        break;
                    }
                }

                if (Found)
                {
                    return (IntPtr)(PIndex + offset);
                }
            }

            return IntPtr.Zero;
        }

        public IntPtr FindPattern(IntPtr module, string pattern, int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return IntPtr.Zero;
            }

            List<byte> patterns = new List<byte>();

            pattern.Split(' ').All((X) =>
            {
                if (X.Equals("?"))
                {
                    patterns.Add(0);
                }
                else
                {
                    patterns.Add(Convert.ToByte(X, 16));
                }

                return true;
            });

            return FindPattern(module, patterns.ToArray(), offset);
        }
    }
}