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
        private readonly IMemory m_Memory;
        #endregion

        public PatternManager(IMemory memory) => m_Memory = memory;

        public IntPtr FindPattern(string module, byte[] pattern, int offset = 0)
        {
            if (pattern == null)
            {
                return IntPtr.Zero;
            }

            ProcessModule moduleInfo = m_Memory.ProcessMemory.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.ModuleName == module);

            if (moduleInfo == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            return FindPattern(moduleInfo, pattern, offset);
        }

        public IntPtr FindPattern(string module, string pattern, int offset = 0) => FindPattern(module, GetPattern(pattern), offset);

        public IntPtr FindPattern(IntPtr modulePtr, byte[] pattern, int offset = 0)
        {
            if (pattern == null)
            {
                return IntPtr.Zero;
            }

            ProcessModule moduleInfo = m_Memory.ProcessMemory.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.BaseAddress == modulePtr);

            if (moduleInfo == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            return FindPattern(moduleInfo, pattern, offset);
        }

        public IntPtr FindPattern(IntPtr modulePtr, string pattern, int offset = 0) => FindPattern(modulePtr, GetPattern(pattern), offset);

        private byte[] GetPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            List<byte> patternsBytes = new List<byte>();

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

        private IntPtr FindPattern(ProcessModule moduleInfo, byte[] pattern, int offset = 0)
        {
            long indexMax = moduleInfo.BaseAddress.ToInt64() + moduleInfo.ModuleMemorySize;

            for (long PIndex = moduleInfo.BaseAddress.ToInt64(); PIndex < indexMax; PIndex++)
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
    }
}