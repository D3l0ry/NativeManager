using System.Linq;
using System.MemoryInteractions;
using System.Runtime.InteropServices;
using System.Text;
using System.WinApi;

namespace System.Diagnostics
{
    public static unsafe class ProcessExtensions
    {
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_DOS_HEADER
        {
            readonly short e_magic;
            readonly short e_cblp;
            readonly short e_cp;
            readonly short e_crlc;
            readonly short e_cparhdr;
            readonly short e_minalloc;
            readonly short e_maxalloc;
            readonly short e_ss;
            readonly short e_sp;
            readonly short e_csum;
            readonly short e_ip;
            readonly short e_cs;
            readonly short e_lfarlc;
            readonly short e_ovno;
            fixed short e_res[4];
            readonly short e_oemid;
            readonly short e_oeminfo;
            fixed short e_res2[10];
            public short e_lfanew;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_NT_HEADERS
        {
            readonly int Signature;
            readonly IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER OptionalHeader;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_FILE_HEADER
        {
            readonly short Machine;
            readonly short NumberOfSections;
            readonly int TimeDataStamp;
            readonly int PointerToSymbolTable;
            readonly int NumberOfSymbols;
            readonly short SizeOfOptionalHeader;
            readonly short Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_OPTIONAL_HEADER
        {
            readonly short Magic;
            readonly byte MajorLinkedVersion;
            readonly byte MinorLinkedVersion;
            readonly int SizeOfCode;
            readonly int SizeOfInitializedData;
            readonly int SizeOfUInitializedData;
            readonly int AddressOfEntryPoint;
            readonly int BaseOfCode;
            readonly int BaseOfData;
            readonly int ImageBase;
            readonly int SectionAlignment;
            readonly int FileAlignment;
            readonly short MajorOperatingSystemVersion;
            readonly short MinororOperatingSystemVersion;
            readonly short MajorImageVersion;
            readonly short MinorImageVersion;
            readonly short MajorSubsystemVersion;
            readonly short MinorSubsystemVersion;
            readonly int Win32VersionValue;
            readonly int SizeOfImage;
            readonly int SizeOfHeaders;
            readonly int CheckSum;
            readonly short Subsystem;
            readonly short DllCharacteristics;
            readonly int SizeOfStackReverse;
            readonly int SizeOfStackCommit;
            readonly int SizeOfHeapReserve;
            readonly int SizeOfHeapCommit;
            readonly int LoaderFlags;
            readonly int NumberOfRvaAndSizes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_DATA_DIRECTORY
        {
            public int VirtualAddress;
            public int Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_EXPORT_DIRECTORY
        {
            readonly int Characteristics;
            readonly int TimeDateStamp;
            readonly short MajorVersion;
            readonly short MinorVersion;
            readonly int Name;
            readonly int Base;
            public int NumberOfFunctions;
            public int NumberOfNames;
            public int AddressOfFunctions;
            public int AddressOfNames;
            public int AddressOfNameOrdinals;
        }

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
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

            ProcessModule selectedModule = process.GetModule(moduleName);

            if (selectedModule is null)
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
        public static PatternManager GetPatternManager(this Process process)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            MemoryManager memoryManager = process.GetMemoryManager();

            return new PatternManager(process, memoryManager);
        }

        /// <summary>
        /// Получает модуль процесса
        /// </summary>
        /// <param name="process">Процесс, из которого нужно получить модуль</param>
        /// <param name="moduleName">Имя модуля</param>
        /// <returns></returns>
        public static ProcessModule GetModule(this Process process, string moduleName)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }


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
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

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
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

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
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            ProcessModule module = process.GetModule(moduleName);

            if (module is null)
            {
                throw new DllNotFoundException("Could not find library at given address.");
            }

            return GetModuleFunctions(process, module.BaseAddress);
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="process">Выбранный процесс</param>
        /// <param name="hModule">Адрес модуля</param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this Process process, IntPtr hModule)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

            MemoryManager memory = process.GetMemoryManager();

            IMAGE_DOS_HEADER dosHeader = memory.Read<IMAGE_DOS_HEADER>(hModule);

            IMAGE_NT_HEADERS ntHeader = memory.Read<IMAGE_NT_HEADERS>(hModule + dosHeader.e_lfanew);

            IMAGE_EXPORT_DIRECTORY exportDirectory = memory.Read<IMAGE_EXPORT_DIRECTORY>(hModule + ntHeader.OptionalHeader.DataDirectory[0].VirtualAddress);

            ModuleInformation[] functions = new ModuleInformation[exportDirectory.NumberOfNames];

            for (int index = 0; index < exportDirectory.NumberOfNames; index++)
            {
                string functionName = Encoding.UTF8.GetString(memory.ReadBytes((IntPtr)((uint)hModule + (uint)memory.Read<IntPtr>(hModule + (exportDirectory.AddressOfNames + index * 0x4))), X => X == 0).ToArray());

                IntPtr functionAddress = (IntPtr)((uint)hModule + (uint)memory.Read<IntPtr>(hModule + (exportDirectory.AddressOfFunctions + index * 0x4)));

                functions[index] = new ModuleInformation(functionName, functionAddress);
            }

            return new ModuleFunctionCollection(functions);
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="moduleManager"></param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this ModuleManager moduleManager)
        {
            IntPtr moduleAddress = moduleManager.Module.BaseAddress;

            IMAGE_DOS_HEADER dosHeader = moduleManager.Read<IMAGE_DOS_HEADER>(moduleAddress);

            IMAGE_NT_HEADERS ntHeader = moduleManager.Read<IMAGE_NT_HEADERS>(moduleAddress + dosHeader.e_lfanew);

            IMAGE_EXPORT_DIRECTORY exportDirectory = moduleManager.Read<IMAGE_EXPORT_DIRECTORY>(moduleAddress + ntHeader.OptionalHeader.DataDirectory[0].VirtualAddress);

            ModuleInformation[] functions = new ModuleInformation[exportDirectory.NumberOfNames];

            for (int index = 0; index < exportDirectory.NumberOfNames; index++)
            {
                byte[] functionNameBytes = moduleManager
                    .ReadBytes((IntPtr)((uint)moduleAddress + (uint)moduleManager.Read<IntPtr>(moduleAddress + (exportDirectory.AddressOfNames + index * 0x4))), X => X != 0);

                string functionName = Encoding.UTF8.GetString(functionNameBytes);

                IntPtr functionAddress = (IntPtr)((uint)moduleAddress + (uint)moduleManager.Read<IntPtr>(moduleAddress + (exportDirectory.AddressOfFunctions + index * 0x4)));

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
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (process.HasExited)
            {
                throw new ApplicationException($"Процесс {process.ProcessName} является завершенным");
            }

            TException exception = typeof(TException)
                .GetConstructor(new Type[] { typeof(string) })
                .Invoke(new object[] { message }) as TException;

            PageManager pageManager = process.GetPageManager();

            MEMORY_BASIC_INFORMATION pageInformation = pageManager.GetPageInformation(address);

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

        /// <summary>
        /// Проверяет, активно ли главное окно процесса
        /// </summary>
        /// <param name="process">Процесс, у которого проверяется окно</param>
        /// <returns></returns>
        public static bool IsActiveWindow(this Process process) => process.MainWindowHandle == User32.GetForegroundWindow();
    }
}