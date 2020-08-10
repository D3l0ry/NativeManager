using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace System.MemoryInteraction
{
    public unsafe class PatternManager
    {
        private readonly Process m_Process;
        private readonly IMemory m_Memory;

        public PatternManager(Process process, IMemory memory)
        {
            m_Process = process;
            m_Memory = memory;
        }

        public IntPtr FindPattern(string module, byte[] pattern, int offset = 0)
        {
            ProcessModule moduleInfo = m_Process.GetModule(module);

            return FindPattern(moduleInfo, pattern, offset);
        }

        public IntPtr FindPattern(string module, string pattern, int offset = 0) => FindPattern(module, GetPattern(pattern), offset);

        public IntPtr FindPattern(ProcessModule moduleInfo, byte[] pattern, int offset = 0)
        {
            if (moduleInfo is null) throw new ArgumentNullException(nameof(moduleInfo));

            return FindPattern(moduleInfo.BaseAddress, (IntPtr)moduleInfo.ModuleMemorySize, pattern, offset);
        }

        public IntPtr FindPattern(ProcessModule moduleInfo, string pattern, int offset = 0) => FindPattern(moduleInfo, GetPattern(pattern), offset);

        public virtual IntPtr FindPattern(IntPtr startAddress, IntPtr endAddress, byte[] pattern, int offset)
        {
            if (pattern is null) throw new ArgumentNullException(nameof(pattern));

            long indexMax = startAddress.ToInt64() + endAddress.ToInt64();

            for (long PIndex = startAddress.ToInt64(); PIndex < indexMax; PIndex++)
            {
                bool Found = true;

                for (int MIndex = 0; MIndex < pattern.Length; MIndex++)
                {
                    Found = pattern[MIndex] == 0 || m_Memory.Read<byte>((IntPtr)(PIndex + MIndex)) == pattern[MIndex];

                    if (!Found) break;
                }

                if (Found) return (IntPtr)(PIndex + offset);
            }

            return IntPtr.Zero;
        }

        public IntPtr FindPattern(IntPtr startAddress, IntPtr endAddress, string pattern, int offset) => FindPattern(startAddress, endAddress, GetPattern(pattern), offset);

        private byte[] GetPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentNullException(nameof(pattern));

            List<byte> patternsBytes = new List<byte>(pattern.Length);

            List<string> patternsStrings = pattern.Split(' ').ToList();
            patternsStrings.RemoveAll(str => str == "");

            foreach (string str in patternsStrings)
            {
                if (str == "?") patternsBytes.Add(0);
                else patternsBytes.Add(Convert.ToByte(str, 16));
            }

            return patternsBytes.ToArray();
        }
    }
}