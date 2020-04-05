using System;
using System.Collections.Generic;
using System.Text;

namespace NativeManager.WinApi.Enums
{
    [Flags]
    public enum FreeType
    {
        MEM_COALESCE_PLACEHOLDERS = 1,
        MEM_PRESERVE_PLACEHOLDER = 2,
        MEM_DECOMMIT = 0x4000,
        MEM_RELEASE = 0x8000
    }
}