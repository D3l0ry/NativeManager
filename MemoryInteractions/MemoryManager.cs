using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.MemoryInteraction
{
    /// <summary>
    /// Класс для работы с виртуальной памятью всего процесса
    /// </summary>
    public sealed unsafe class MemoryManager : ModuleManager, IMemory
    {
        #region Private variables
        private readonly Lazy<List<ModuleManager>> m_ProcessModules = new Lazy<List<ModuleManager>>();
        #endregion

        #region Initialization
        public MemoryManager(Process process) : base(process, IntPtr.Zero)
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
                ModuleManager selectedModule = m_ProcessModules.Value.Find(X => X.ModuleName == moduleName);

                if(selectedModule is null)
                {
                    ModuleManager newModule = new ModuleManager(m_Process, moduleName);

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
                ModuleManager selectedModule = m_ProcessModules.Value.Find(X => X.ModulePtr == modulePtr);

                if (selectedModule is null)
                {
                    ModuleManager newModule = new ModuleManager(m_Process, modulePtr);

                    m_ProcessModules.Value.Add(newModule);

                    return m_ProcessModules.Value.Last();
                }

                return selectedModule;
            }
        }
        #endregion

        /// <summary>
        /// Получает объект класса для работы с памятью текущего процесса
        /// </summary>
        /// <returns></returns>
        public static MemoryManager GetCurrentProcessMemory() => new MemoryManager(Process.GetCurrentProcess());
    }
}