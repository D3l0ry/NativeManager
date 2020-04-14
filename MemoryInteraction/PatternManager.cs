using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using NativeManager.MemoryInteraction.Interfaces;
using NativeManager.ProcessInteraction;

namespace NativeManager.MemoryInteraction
{
    public unsafe class PatternManager
    {
        #region Private variables
        private readonly IMemory m_Memory;
        #endregion

        public PatternManager(IMemory memory) => m_Memory = memory;

        public virtual IntPtr FindPattern(string module, byte[] pattern, int offset = 0)
        {
            ProcessModule moduleInfo = ProcessInfo.GetModule(m_Memory.ProcessMemory, module);

            return FindPattern(moduleInfo, pattern, offset);
        }

        public virtual IntPtr FindPattern(string module, string pattern, int offset = 0) => FindPattern(module, GetPattern(pattern), offset);

        public virtual IntPtr FindPattern(IntPtr modulePtr, byte[] pattern, int offset = 0)
        {
            ProcessModule moduleInfo = ProcessInfo.GetModule(m_Memory.ProcessMemory, modulePtr);

            return FindPattern(moduleInfo, pattern, offset);
        }

        public virtual IntPtr FindPattern(IntPtr modulePtr, string pattern, int offset = 0) => FindPattern(modulePtr, GetPattern(pattern), offset);

        public virtual IntPtr FindPattern(ProcessModule moduleInfo, byte[] pattern, int offset = 0)
        {
            if(moduleInfo == null)
            {
                throw new ArgumentNullException("moduleInfo");
            }

            return FindPattern(moduleInfo.BaseAddress, pattern, moduleInfo.ModuleMemorySize, offset);
        }

        public virtual IntPtr FindPattern(ProcessModule moduleInfo, string pattern, int offset = 0) => FindPattern(moduleInfo, GetPattern(pattern), offset);

        public virtual IntPtr FindPattern(IntPtr modulePtr, byte[] pattern, int searchSize = 5000, int offset = 0)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            long indexMax = modulePtr.ToInt64() + searchSize;

            for (long PIndex = modulePtr.ToInt64(); PIndex < indexMax; PIndex++)
            {
                bool Found = true;

                for (int MIndex = 0; MIndex < pattern.Length; MIndex++)
                {
                    Found = pattern[MIndex] == 0 || m_Memory.Read<byte>((IntPtr)(PIndex + MIndex)) == pattern[MIndex];

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

        public virtual IntPtr FindPattern(IntPtr modulePtr, string pattern, int searchSize = 5000, int offset = 0) => FindPattern(modulePtr, GetPattern(pattern), searchSize, offset);

        private byte[] GetPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            List<byte> patternsBytes = new List<byte>(pattern.Length);

            List<string> patternsStrings = pattern.Split(' ').ToList();
            patternsStrings.RemoveAll(str => str == "");

            foreach (string str in patternsStrings)
            {
                if (str == "?")
                {
                    patternsBytes.Add(0);
                }
                else
                {
                    patternsBytes.Add(Convert.ToByte(str, 16));
                }
            }

            return patternsBytes.ToArray();
        }
    }
}