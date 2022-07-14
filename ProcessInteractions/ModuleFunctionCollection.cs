using System.Collections;
using System.Linq;

namespace System.Diagnostics
{
    public class ModuleFunctionCollection : ReadOnlyCollectionBase
    {
        public ModuleFunctionCollection(ModuleInformation[] moduleFunctions) => InnerList.AddRange(moduleFunctions);

        public ModuleInformation this[int index] => InnerList[index] as ModuleInformation;

        public ModuleInformation this[string name] => InnerList.Cast<ModuleInformation>().Where(X => X.Name == name).FirstOrDefault();
    }
}