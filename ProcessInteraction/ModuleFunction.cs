namespace System.Diagnostics
{
    public class ModuleFunction:ModuleAddress
    {
        internal ModuleFunction(string name, IntPtr virtualAddress):base(name, virtualAddress) { }
    }
}