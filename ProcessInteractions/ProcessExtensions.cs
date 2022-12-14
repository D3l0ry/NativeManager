using System.Linq;
using System.MemoryInteractions;
using System.Text;
using System.WinApi;

namespace System.Diagnostics
{
    public static unsafe partial class ProcessExtensions
    {
        /// <summary>
        /// Получает экземпляр класса для работы с виртуальной памятью
        /// </summary>
        /// <param name="process">Процесс для работы с виртуальной памятью</param>
        /// <returns></returns>
        public static SimpleMemoryManager GetSimpleMemoryManager(this Process process) => new SimpleMemoryManager(process);

        /// <summary>
        /// Получает экземпляр класса для работы с виртуальной памятью
        /// </summary>
        /// <param name="process">Процесс для работы с виртуальной памятью</param>
        /// <returns></returns>
        public static MemoryManager GetMemoryManager(this Process process) => new MemoryManager(process);

        /// <summary>
        /// Получает экземпляр класса для работы с виртуальной памятью выбранного модуля
        /// </summary>
        /// <param name="process">Процесс для работы с виртуальной памятью</param>
        /// <param name="moduleName">Выбранный модуль процесса</param>
        /// <returns></returns>
        public static ModuleManager GetModuleManager(this Process process, string moduleName)
        {
            ProcessModule selectedModule = process.GetModule(moduleName);

            if (selectedModule == null)
            {
                throw new NullReferenceException($"Модуль под именем {moduleName} не найден!");
            }

            return new ModuleManager(process, selectedModule);
        }

        /// <summary>
        /// Получает экземпляр класса для работы с виртуальной памятью выбранного модуля
        /// </summary>
        /// <param name="process">Процесс для работы с виртуальной памятью</param>
        /// <param name="moduleName">Выбранный модуль процесса</param>
        /// <returns></returns>
        public static ModuleManager GetModuleManager(this Process process, IntPtr moduleBaseAddress)
        {
            ProcessModule selectedModule = process.GetModule(moduleBaseAddress);

            if (selectedModule == null)
            {
                throw new NullReferenceException("Модуль процесса не найден!");
            }

            return new ModuleManager(process, selectedModule);
        }

        /// <summary>
        /// Получает экземпляр класса для работы с виртуальной памятью
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static Allocator GetAllocator(this Process process) => new Allocator(process);

        /// <summary>
        /// Получает экземпляр класса для выполнения кода процесса
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static Executor GetExecutor(this Process process) => new Executor(process);

        /// <summary>
        /// Получает экземпляр класса для работы со страницами процесса
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static PageManager GetPageManager(this Process process) => new PageManager(process);

        /// <summary>
        /// Получает экземпляр класса для работы с паттернами
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static PatternManager GetPatternManager(this Process process) => new PatternManager(process, process.GetMemoryManager());

        /// <summary>
        /// Получает модуль процесса
        /// </summary>
        /// <param name="process">Процесс, из которого нужно получить модуль</param>
        /// <param name="moduleName">Имя модуля</param>
        /// <returns></returns>
        public static ProcessModule GetModule(this Process process, string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            ProcessModule processModule = process.Modules
                .Cast<ProcessModule>()
                .FirstOrDefault(currentModule => currentModule.ModuleName == moduleName);

            return processModule;
        }

        /// <summary>
        /// Получает модуль процесса
        /// </summary>
        /// <param name="process">Процесс, из которого нужно получить модуль</param>
        /// <param name="hModule">Базовый адрес модуля</param>
        /// <returns></returns>
        public static ProcessModule GetModule(this Process process, IntPtr hModule)
        {
            ProcessModule processModule = process.Modules
                .Cast<ProcessModule>()
                .FirstOrDefault(currentModule => currentModule.BaseAddress == hModule);

            return processModule;
        }

        /// <summary>
        /// Получает список адресов модулей выбранного процесса
        /// </summary>
        /// <param name="process">Процесс, из которого нужно получить адреса модулей</param>
        /// <returns></returns>
        public static ModuleInformationCollection GetModulesAddress(this Process process)
        {
            ProcessModuleCollection moduleCollection = process.Modules;
            ModuleInformation[] addresses = new ModuleInformation[moduleCollection.Count];

            for (int index = 0; index < moduleCollection.Count; index++)
            {
                ProcessModule module = moduleCollection[index];

                addresses[index] = new ModuleInformation(module.ModuleName, module.BaseAddress);
            }

            return new ModuleInformationCollection(addresses);
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="process">Выбранный процесс</param>
        /// <param name="moduleName">Имя модуля</param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this Process process, string moduleName)
        {
            ModuleManager moduleManager = process.GetModuleManager(moduleName);

            return moduleManager.GetModuleFunctions();
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="process">Выбранный процесс</param>
        /// <param name="hModule">Адрес модуля</param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this Process process, IntPtr hModule)
        {
            ModuleManager moduleManager = process.GetModuleManager(hModule);

            return moduleManager.GetModuleFunctions();
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="moduleManager"></param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this ModuleManager moduleManager)
        {
            ImageDosHeader dosHeader = moduleManager.Read<ImageDosHeader>(IntPtr.Zero);
            ImageNtHeaders ntHeader = moduleManager.Read<ImageNtHeaders>((IntPtr)dosHeader.e_lfanew);
            ImageExportDirectory exportDirectory = moduleManager.Read<ImageExportDirectory>((IntPtr)ntHeader.OptionalHeader.DataDirectory[0].VirtualAddress);
            ModuleInformation[] functions = new ModuleInformation[exportDirectory.NumberOfNames];

            for (int index = 0; index < exportDirectory.NumberOfNames; index++)
            {
                string functionName = Encoding.UTF8
                    .GetString(moduleManager.ReadBytes(moduleManager.Read<IntPtr>((IntPtr)(exportDirectory.AddressOfNames + index * 0x4)), X => X == 0)
                .ToArray());

                IntPtr functionAddress = (IntPtr)((uint)moduleManager.Module.BaseAddress + (uint)moduleManager.Read<IntPtr>((IntPtr)(exportDirectory.AddressOfFunctions + index * 0x4)));

                functions[index] = new ModuleInformation(functionName, functionAddress);
            }

            return new ModuleFunctionCollection(functions);
        }

        /// <summary>
        /// Вызывает Exception определенного типа с сообщением и заполненным словарем с информацией об указанном адресе в памяти
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="process"></param>
        /// <param name="address"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static TException ShowException<TException>(this Process process, IntPtr address, string message) where TException : Exception
        {
            TException exception = typeof(TException)
                .GetConstructor(new Type[] { typeof(string) })
                .Invoke(new object[] { message }) as TException;

            PageManager pageManager = process.GetPageManager();
            MemoryBasicInformation pageInformation = pageManager.GetPageInformation(address);

            exception.Data.Add("Process", process.ProcessName);
            exception.Data.Add("Address", address);
            exception.Data.Add("BaseAddress", pageInformation.BaseAddress);
            exception.Data.Add("AllocationProtect", pageInformation.AllocationProtect);
            exception.Data.Add("RegionSize", pageInformation.RegionSize);
            exception.Data.Add("StateType", pageInformation.State);
            exception.Data.Add("CurrentProtect", pageInformation.Protect);
            exception.Data.Add("MemoryType", pageInformation.Type);

            return exception;
        }

        internal static void CheckProcess(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }
        }

        /// <summary>
        /// Проверяет, активно ли главное окно процесса
        /// </summary>
        /// <param name="process">Процесс, у которого проверяется окно</param>
        /// <returns></returns>
        public static bool IsActiveWindow(this Process process) => process.MainWindowHandle == User32.GetForegroundWindow();
    }
}