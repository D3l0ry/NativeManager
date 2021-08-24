namespace System.Diagnostics
{
    public class ModuleFunction : ModuleInformation
    {
        internal ModuleFunction(string name, IntPtr virtualAddress) : base(name, virtualAddress) { }
    }
}