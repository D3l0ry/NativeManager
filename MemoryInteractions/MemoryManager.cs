using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.MemoryInteraction
{
    /// <summary>
    /// Класс для работы с виртуальной памятью всего процесса
    /// </summary>
    public sealed unsafe class MemoryManager : ModuleManager, IMemory
    {
        #region Private variables
        private readonly Lazy<List<ModuleManager>> m_ProcessModules;
        #endregion

        #region Initialization
        public MemoryManager(Process process) : base(process, process.MainModule)
        {
            m_ProcessModules = new Lazy<List<ModuleManager>>();
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Получает объект класса ModuleManager для работы с адресами выбранного модуля
        /// </summary>
        /// <value></value>
        public ModuleManager this[string moduleName]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Value.FirstOrDefault(X => X.ModuleName == moduleName);

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
                ModuleManager selectedModule = m_ProcessModules.Value.FirstOrDefault(X => X.ModulePtr == modulePtr);

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
        #endregion

        protected override IntPtr TryGetNewAddress(IntPtr address, int size) => address;

        /// <summary>
        /// Получает объект класса для работы с памятью текущего процесса
        /// </summary>
        /// <returns></returns>
        public static MemoryManager GetCurrentProcessMemory() => new MemoryManager(Process.GetCurrentProcess());
    }
}