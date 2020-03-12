using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NativeManager.WinApi;

namespace NativeManager.ProcessManager
{
    public unsafe static class ProcessInfo
    {
        public static Process GetProcess(this Process[] process, int index = 0)
        {
            Found(process, index);

            return process[index];
        }

        public static Process GetProcess(string processName, int index = 0) => GetProcess(Process.GetProcessesByName(processName),index);

        public static bool IsProcessActive(this Process[] process) => process.Length > 0 ? true : false;

        public static bool IsProcessActive(string processName) => IsProcessActive(Process.GetProcessesByName(processName));

        public static bool IsProcessActiveWindow(this Process process) => process.MainWindowHandle == User32.GetForegroundWindow() ? true : false;

        public static bool IsProcessActiveWindow(this string processName) => GetProcess(processName).MainWindowHandle == User32.GetForegroundWindow() ? true : false;

        public static ProcessModule GetModule(this Process process, string module) => process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.ModuleName == module);

        public static Dictionary<string, ProcessModule> GetModules(this Process process)
        {
            Dictionary<string, ProcessModule> Modules = new Dictionary<string, ProcessModule>();

            if (process.Modules.Count == 0)
            {
                throw new IndexOutOfRangeException("Modules equals zero.");
            }

            process.Modules.Cast<ProcessModule>().All(mdl =>
            {
                Modules.Add(mdl.ModuleName, mdl);

                return true;
            });

            return Modules;
        }

        public static Dictionary<string, IntPtr> GetAddressModules(this Process process)
        {
            Dictionary<string, IntPtr> Modules = new Dictionary<string, IntPtr>();

            if (process.Modules.Count == 0)
            {
                throw new IndexOutOfRangeException("Modules equals zero.");
            }

            process.Modules.Cast<ProcessModule>().All(mdl =>
            {
                Modules.Add(mdl.ModuleName, mdl.BaseAddress);

                return true;
            });

            return Modules;
        }

        public static Dictionary<string, IntPtr> GetAddressModules(this Process[] process, int index = 0)
        {
            Found(process, index);

            return GetAddressModules(process[index]);
        }

        public static Dictionary<string, IntPtr> GetAddressModules(this string processName, int index = 0)
        {
            Process[] process = Process.GetProcessesByName(processName);

            Found(process, index);

            return GetAddressModules(process[index]);
        }

        public static void Found(this Process[] process, int index)
        {
            if (process.Length != 0 && index <= (process.Length - 1))
            {
                return;
            }

            throw new InvalidOperationException("Process not found");
        }
    }
}