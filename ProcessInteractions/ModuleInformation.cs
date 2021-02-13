namespace System.Diagnostics
{
    public class ModuleInformation
    {
        internal ModuleInformation(string name, IntPtr virtualAddress)
        {
            Name = name;
            VirtualAddress = virtualAddress;
        }

        public string Name;
        public IntPtr VirtualAddress;
    }
}