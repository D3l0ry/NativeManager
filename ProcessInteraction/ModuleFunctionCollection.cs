using System.Collections;
using System.Linq;

namespace System.Diagnostics
{
    public class ModuleFunctionCollection:ReadOnlyCollectionBase
    {
        public ModuleFunctionCollection(ModuleFunction[] moduleFunctions) => InnerList.AddRange(moduleFunctions);

        public ModuleFunction this[int index] => InnerList[index] as ModuleFunction;

        public ModuleFunction this[string name] => InnerList.Cast<ModuleFunction>().Where(X => X.Name == name).FirstOrDefault();
    }
}