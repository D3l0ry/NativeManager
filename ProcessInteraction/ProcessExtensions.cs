using System.Collections.Generic;
using System.Linq;

using System.MemoryInteraction;
using System.WinApi;

namespace System.Diagnostics
{
    public static class ProcessExtensions
    {
        public static MemoryManager GetMemoryManager(this Process process) => new MemoryManager(process);

        public static SimpleMemoryManager GetSimpleMemoryManager(this Process process) => new SimpleMemoryManager(process);

        public static ProcessModule GetModule(this Process process, string module)
        {
            if(string.IsNullOrWhiteSpace(module))
            {
                throw new ArgumentNullException("module");
            }

            if (process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            var processModule = process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.ModuleName == module);

            if (processModule == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            return processModule;
        }

        public static ProcessModule GetModule(this Process process, IntPtr modulePtr)
        {
            if (process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            var processModule = process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.BaseAddress == modulePtr);

            if (processModule == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            return processModule;
        }

        public static Dictionary<string, ProcessModule> GetModules(this Process process)
        {
            Dictionary<string, ProcessModule> Modules = new Dictionary<string, ProcessModule>();

            if (process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero");
            }

            process.Modules.Cast<ProcessModule>().All(mdl =>
            {
                Modules.Add(mdl.ModuleName, mdl);

                return true;
            });

            return Modules;
        }

        public static Dictionary<string, IntPtr> GetModulesAddress(this Process process)
        {
            Dictionary<string, IntPtr> Modules = new Dictionary<string, IntPtr>();

            foreach (var module in GetModules(process))
            {
                Modules.Add(module.Key, module.Value.BaseAddress);
            }

            return Modules;
        }

        public static bool IsActiveWindow(this Process process) => process.MainWindowHandle == User32.GetForegroundWindow();
    }
}