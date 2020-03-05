using System;
using System.Collections.Generic;
using System.Diagnostics;

using NativeManager.WinApi;

namespace NativeManager.ProcessManager
{
    public unsafe static class ProcessInfo
    {
        public static Process GetProcess(this Process[] process, int index = 0)
        {
            Found(process, index);

            return process?[index];
        }

        public static Process GetProcess(string processName, int index = 0) => GetProcess(Process.GetProcessesByName(processName));

        public static bool IsProcessActive(this Process[] process) => process.Length > 0 ? true : false;

        public static bool IsProcessActive(string processName) => IsProcessActive(Process.GetProcessesByName(processName));

        public static bool IsProcessActiveWindow(this Process process) => process.MainWindowHandle == User32.GetForegroundWindow() ? true : false;

        public static bool IsProcessActiveWindow(this string processName) => (GetProcess(processName)?.MainWindowHandle ?? IntPtr.Zero) == User32.GetForegroundWindow() ? true : false;

        public static Dictionary<string, IntPtr> GetProcessModule(this Process process)
        {
            Dictionary<string, IntPtr> Modules = new Dictionary<string, IntPtr>();

            foreach (ProcessModule UIModule in process.Modules)
            {
                Modules.Add(UIModule.ModuleName, UIModule.BaseAddress);
            }

            return Modules;
        }

        public static Dictionary<string, IntPtr> GetProcessModule(this Process[] process, int index = 0)
        {
            Found(process, index);

            return GetProcessModule(process[index]);
        }

        public static Dictionary<string, IntPtr> GetProcessModule(this string processName, int index = 0) => GetProcessModule(Process.GetProcessesByName(processName)[index]);

        public static void Found(Process[] process, int index)
        {
            if (process.Length != 0 && index <= (process.Length - 1))
            {
                return;
            }

            throw new InvalidOperationException("Process not found");
        }
    }
}