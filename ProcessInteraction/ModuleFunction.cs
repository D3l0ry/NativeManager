namespace System.Diagnostics
{
    public class ModuleFunction
    {
        internal ModuleFunction(string name, IntPtr virtualAddress)
        {
            Name = name;
            VirtualAddress = virtualAddress;
        }

        public string Name;
        public IntPtr VirtualAddress;
    }
}