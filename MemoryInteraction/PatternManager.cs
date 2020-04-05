using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using NativeManager.MemoryInteraction.Interfaces;

namespace NativeManager.MemoryInteraction
{
    public unsafe class PatternManager
    {
        #region Private variables
        private readonly IMemory m_MemoryManager;
        #endregion

        public PatternManager(IMemory memory) => m_MemoryManager = memory;

        public IntPtr FindPattern(IntPtr modulePtr, byte[] pattern, int offset = 0)
        {
            if (pattern == null)
            {
                return IntPtr.Zero;
            }

            ProcessModule moduleInfo = m_MemoryManager.ProcessMemory.Modules.Cast<ProcessModule>().FirstOrDefault(module => module.BaseAddress == modulePtr);

            if(moduleInfo == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            long indexMax = modulePtr.ToInt64() + moduleInfo.ModuleMemorySize;

            for (long PIndex = modulePtr.ToInt64(); PIndex < indexMax; PIndex++)
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

        public IntPtr FindPattern(IntPtr modulePtr, string pattern, int offset = 0)
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

            return FindPattern(modulePtr, patterns.ToArray(), offset);
        }
    }
}