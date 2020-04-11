using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NativeManager.WinApi;

namespace NativeManager.ProcessInteraction
{
    public unsafe class ProcessInfo
    {
        #region Private variables
        private readonly Process m_Process;
        #endregion

        public ProcessInfo(Process process) => m_Process = process;

        public ProcessModule GetModule(string module)
        {
            if (m_Process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            return m_Process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.ModuleName == module);
        }

        public Dictionary<string, ProcessModule> GetModules()
        {
            Dictionary<string, ProcessModule> Modules = new Dictionary<string, ProcessModule>();

            if (m_Process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            m_Process.Modules.Cast<ProcessModule>().All(mdl =>
            {
                Modules.Add(mdl.ModuleName, mdl);

                return true;
            });

            return Modules;
        }

        public Dictionary<string, IntPtr> GetAddressModules()
        {
            Dictionary<string, IntPtr> Modules = new Dictionary<string, IntPtr>();

            if (m_Process.Modules.Count == 0)
            {
                throw new InvalidOperationException("Modules equals zero.");
            }

            m_Process.Modules.Cast<ProcessModule>().All(mdl =>
            {
                Modules.Add(mdl.ModuleName, mdl.BaseAddress);

                return true;
            });

            return Modules;
        }

        public bool IsActiveWindow() => m_Process.MainWindowHandle == User32.GetForegroundWindow();

        internal static void Exists(Process[] process, int index = 0)
        {
            if (process.Length != 0 && index <= (process.Length - 1))
            {
                return;
            }

            throw new InvalidOperationException("Process not found");
        }
    }
}