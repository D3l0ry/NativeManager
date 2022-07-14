using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.MemoryInteractions
{
    /// <summary>
    /// Класс для работы с виртуальной памятью всего процесса
    /// </summary>
    public sealed unsafe class MemoryManager : ModuleManager
    {
        private readonly Lazy<List<ModuleManager>> m_ProcessModules;

        public MemoryManager(Process process) : base(process, process.MainModule) => m_ProcessModules = new Lazy<List<ModuleManager>>();

        /// <summary>
        /// Получает объект класса ModuleManager для работы с адресами выбранного модуля
        /// </summary>
        /// <value></value>
        public ModuleManager this[string moduleName]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Value
                    .FirstOrDefault(currentModule => currentModule.Module.ModuleName == moduleName);

                if (selectedModule is null)
                {
                    ProcessModule module = m_Process.GetModule(moduleName);

                    if (module is null)
                    {
                        throw new NullReferenceException("Модуль процесса не найден!");
                    }

                    ModuleManager newModule = new ModuleManager(m_Process, module);

                    m_ProcessModules.Value.Add(newModule);

                    return m_ProcessModules.Value.Last();
                }

                return selectedModule;
            }
        }

        /// <summary>
        /// Получает объект класса ModuleManager для работы с адресами выбранного модуля
        /// </summary>
        /// <value></value>
        public ModuleManager this[IntPtr modulePtr]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Value
                    .FirstOrDefault(currentModule => currentModule.Module.BaseAddress == modulePtr);

                if (selectedModule is null)
                {
                    ProcessModule module = m_Process.GetModule(modulePtr);

                    if (module is null)
                    {
                        throw new NullReferenceException("Модуль процесса не найден!");
                    }

                    ModuleManager newModule = new ModuleManager(m_Process, module);

                    m_ProcessModules.Value.Add(newModule);

                    return m_ProcessModules.Value.Last();
                }

                return selectedModule;
            }
        }

        protected override IntPtr TryGetNewAddress(IntPtr address, int size) => address;

        /// <summary>
        /// Получает объект класса для работы с памятью текущего процесса
        /// </summary>
        /// <returns></returns>
        public static MemoryManager GetCurrentProcessMemory()
        {
            Process currentProcess = Process.GetCurrentProcess();

            return new MemoryManager(currentProcess);
        }
    }
}