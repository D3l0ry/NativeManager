using System.Collections;
using System.Linq;

namespace System.Diagnostics
{
    public class ModuleInformationCollection : ReadOnlyCollectionBase
    {
        public ModuleInformationCollection(ModuleInformation[] moduleAddresses) => InnerList.AddRange(moduleAddresses);

        public ModuleInformation this[string name] => InnerList.Cast<ModuleInformation>().Where(X => X.Name == name).FirstOrDefault();
    }
}