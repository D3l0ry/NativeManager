namespace System.Diagnostics
{
    public class ModuleAddress
    {
        internal ModuleAddress(string name, IntPtr virtualAddress)
        {
            Name = name;
            VirtualAddress = virtualAddress;
        }

        public string Name;
        public IntPtr VirtualAddress;
    }
}