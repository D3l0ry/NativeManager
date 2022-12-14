using System.Collections.ObjectModel;
using System.Linq;

namespace System.Diagnostics
{
    public class ModuleInformationCollection : ReadOnlyCollection<ModuleInformation>
    {
        public ModuleInformationCollection(ModuleInformation[] moduleAddresses) : base(moduleAddresses) { }

        public ModuleInformation this[string name] => Items.First(currentModule => currentModule.Name == name);
    }
}