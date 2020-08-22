using System.Collections;
using System.Linq;

namespace System.Diagnostics
{
    public class ModuleAddressCollection :ReadOnlyCollectionBase
    {
        public ModuleAddressCollection(ModuleAddress[] moduleAddresses) => InnerList.AddRange(moduleAddresses);

        public ModuleAddress this[string name] => InnerList.Cast<ModuleAddress>().Where(X => X.Name == name).FirstOrDefault();
    }
}