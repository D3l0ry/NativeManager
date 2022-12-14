using System.Collections.ObjectModel;
using System.Linq;

namespace System.Diagnostics
{
    public class ModuleFunctionCollection : ReadOnlyCollection<ModuleInformation>
    {
        public ModuleFunctionCollection(ModuleInformation[] moduleFunctions) : base(moduleFunctions) { }

        public ModuleInformation this[string name] => Items.First(X => X.Name == name);
    }
}