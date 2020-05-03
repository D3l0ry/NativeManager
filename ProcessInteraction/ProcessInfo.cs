using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NativeManager.MemoryInteraction;
using NativeManager.WinApi;
using NativeManager.WinApi.Enums;

namespace NativeManager.ProcessInteraction
{
    public unsafe class ProcessInfo
    {
        private readonly Process m_Process;

        public ProcessInfo(Process process) => m_Process = process;

        public ProcessModule GetModule(string module)
        {
            if (m_Process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            var processModule = m_Process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.ModuleName == module);

            if (processModule == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            return processModule;
        }

        public ProcessModule GetModule(IntPtr modulePtr)
        {
            if (m_Process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            var processModule = m_Process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.BaseAddress == modulePtr);

            if (processModule == null)
            {
                throw new DllNotFoundException($"Could not find library at given address.");
            }

            return processModule;
        }

        public Dictionary<string, ProcessModule> GetModules()
        {
            Dictionary<string, ProcessModule> Modules = new Dictionary<string, ProcessModule>();

            if (m_Process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero");
            }

            m_Process.Modules.Cast<ProcessModule>().All(mdl =>
            {
                Modules.Add(mdl.ModuleName, mdl);

                return true;
            });

            return Modules;
        }

        public Dictionary<string, IntPtr> GetModulesAddress()
        {
            Dictionary<string, IntPtr> Modules = new Dictionary<string, IntPtr>();

            foreach(var module in GetModules())
            {
                Modules.Add(module.Key, module.Value.BaseAddress);
            }

            return Modules;
        }

        public bool IsActiveWindow() => m_Process.MainWindowHandle == User32.GetForegroundWindow();

        public static MemoryManager[] GetMemoryProcesses(string processName, ProcessAccess access = ProcessAccess.ALL)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            MemoryManager[] memoryes = new MemoryManager[processes.Length];

            for (int index = 0; index < processes.Length; index++)
            {
                memoryes[index] = new MemoryManager(processes[index], access);
            }

            return memoryes;
        }

        public static MemoryManager GetMemoryProcessById(int processId, ProcessAccess access = ProcessAccess.ALL) => new MemoryManager(Process.GetProcessById(processId), access);

        internal static Process Exists(Process[] processes, int index = 0)
        {
            if (processes.Length != 0 && index <= (processes.Length - 1))
            {
                return processes[index];
            }

            throw new InvalidOperationException("Process not found");
        }
    }
}