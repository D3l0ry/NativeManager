using System.Runtime.InteropServices;

namespace System.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageDosHeader
    {
        public readonly short e_magic;
        public readonly short e_cblp;
        public readonly short e_cp;
        public readonly short e_crlc;
        public readonly short e_cparhdr;
        public readonly short e_minalloc;
        public readonly short e_maxalloc;
        public readonly short e_ss;
        public readonly short e_sp;
        public readonly short e_csum;
        public readonly short e_ip;
        public readonly short e_cs;
        public readonly short e_lfarlc;
        public readonly short e_ovno;
        public fixed short e_res[4];
        public readonly short e_oemid;
        public readonly short e_oeminfo;
        public fixed short e_res2[10];
        public short e_lfanew;
    }
}