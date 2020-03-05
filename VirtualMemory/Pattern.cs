using System;
using System.Collections.Generic;
using System.Linq;

using NativeManager.WinApi.Enums;
using NativeManager.WinApi;

namespace NativeManager.VirtualMemory
{
    public unsafe static class Pattern
    {
        public static IntPtr FindPattern(HMemory hMemory, uint module, string pattern, int offset = 0)
        {
            List<byte> Patterns = new List<byte>();

            pattern.Split(' ').All((X) =>
            {
                if (X.Equals("?"))
                {
                    Patterns.Add(0);
                }
                else
                {
                    Patterns.Add(Convert.ToByte(X, 16));
                }

                return true;
            });

            Kernel32.GetModuleInformation(hMemory.Handle, (IntPtr)module, out MODULEINFO MODULEINFO, sizeof(MODULEINFO));

            for (long PIndex = module; PIndex < MODULEINFO.SizeOfImage; PIndex++)
            {
                bool Found = true;

                for (int MIndex = 0; MIndex < Patterns.Count; MIndex++)
                {
                    Found = Patterns[MIndex] == 0 || hMemory.Read<byte>(PIndex + MIndex) == Patterns[MIndex];

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