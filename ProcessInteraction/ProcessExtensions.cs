using System.Collections.Generic;
using System.Linq;

using System.MemoryInteraction;
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
            short e_magic;
            short e_cblp;
            short e_cp;
            short e_crlc;
            short e_cparhdr;
            short e_minalloc;
            short e_maxalloc;
            short e_ss;
            short e_sp;
            short e_csum;
            short e_ip;
            short e_cs;
            short e_lfarlc;
            short e_ovno;
            fixed short e_res[4];
            short e_oemid;
            short e_oeminfo;
            fixed short e_res2[10];
            public short e_lfanew;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_NT_HEADERS
        {
            int Signature;
            IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER OptionalHeader;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_FILE_HEADER
        {
            short Machine;
            short NumberOfSections;
            int TimeDataStamp;
            int PointerToSymbolTable;
            int NumberOfSymbols;
            short SizeOfOptionalHeader;
            short Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IMAGE_OPTIONAL_HEADER
        {
            short Magic;
            byte MajorLinkedVersion;
            byte MinorLinkedVersion;
            int SizeOfCode;
            int SizeOfInitializedData;
            int SizeOfUInitializedData;
            int AddressOfEntryPoint;
            int BaseOfCode;
            int BaseOfData;
            int ImageBase;
            int SectionAlignment;
            int FileAlignment;
            short MajorOperatingSystemVersion;
            short MinororOperatingSystemVersion;
            short MajorImageVersion;
            short MinorImageVersion;
            short MajorSubsystemVersion;
            short MinorSubsystemVersion;
            int Win32VersionValue;
            int SizeOfImage;
            int SizeOfHeaders;
            int CheckSum;
            short Subsystem;
            short DllCharacteristics;
            int SizeOfStackReverse;
            int SizeOfStackCommit;
            int SizeOfHeapReserve;
            int SizeOfHeapCommit;
            int LoaderFlags;
            int NumberOfRvaAndSizes;

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
            int Characteristics;
            int TimeDateStamp;
            short MajorVersion;
            short MinorVersion;
            int Name;
            int Base;
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
        public static SimpleMemoryManager GetSimpleMemoryManager(this Process process)
        {
            if (process is null) throw new ArgumentNullException(nameof(process));

            return new SimpleMemoryManager(process);
        }

        /// <summary>
        /// Получает экземпляр класса для работы с виртуальной памятью
        /// </summary>
        /// <param name="process">Процесс для работы с виртуальной памятью</param>
        /// <returns></returns>
        public static MemoryManager GetMemoryManager(this Process process)
        {
            if (process is null) throw new ArgumentNullException(nameof(process));

            return new MemoryManager(process);
        }

        /// <summary>
        /// Получает модуль процесса
        /// </summary>
        /// <param name="process">Процесс, из которого нужно получить модуль</param>
        /// <param name="moduleName">Имя модуля</param>
        /// <returns></returns>
        public static ProcessModule GetModule(this Process process, string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentNullException(nameof(moduleName));

            ProcessModule processModule = process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.ModuleName == moduleName);

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
            ProcessModule processModule = process.Modules.Cast<ProcessModule>().FirstOrDefault(mdl => mdl.BaseAddress == hModule);

            return processModule;
        }

        /// <summary>
        /// Получает список адресов модулей выбранного процесса
        /// </summary>
        /// <param name="process">Процесс, из которого нужно получить адреса модулей</param>
        /// <returns></returns>
        public static Dictionary<string, IntPtr> GetModulesAddress(this Process process)
        {
            Dictionary<string, IntPtr> Modules = new Dictionary<string, IntPtr>();

            foreach (ProcessModule module in process.Modules)
            {
                Modules.Add(module.ModuleName, module.BaseAddress);
            }

            return Modules;
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="process">Выбранный процесс</param>
        /// <param name="hModule">Адрес модуля</param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this Process process, IntPtr hModule)
        {
            using (MemoryManager memory = process.GetMemoryManager())
            {
                IMAGE_DOS_HEADER dosHeader = memory.Read<IMAGE_DOS_HEADER>(hModule);

                IMAGE_NT_HEADERS ntHeader = memory.ReadManaged<IMAGE_NT_HEADERS>(hModule + dosHeader.e_lfanew);

                IMAGE_EXPORT_DIRECTORY exportDirectory = memory.Read<IMAGE_EXPORT_DIRECTORY>(hModule + ntHeader.OptionalHeader.DataDirectory[0].VirtualAddress);

                ModuleFunction[] functions = new ModuleFunction[exportDirectory.NumberOfNames];

                for (int index = 0; index < exportDirectory.NumberOfNames; index++)
                {
                    functions[index] = new ModuleFunction(Encoding.UTF8.GetString(memory.ReadBytes((IntPtr)((uint)hModule + (uint)memory.Read<IntPtr>(hModule + (exportDirectory.AddressOfNames + index * 0x4))), X => X == 0)), (IntPtr)((uint)hModule + (uint)memory.Read<IntPtr>(hModule + (exportDirectory.AddressOfFunctions + index * 0x4))));
                }

                return new ModuleFunctionCollection(functions);
            }
        }

        /// <summary>
        /// Получает список функций в модуле
        /// </summary>
        /// <param name="process">Выбранный процесс</param>
        /// <param name="moduleName">Имя модуля</param>
        /// <returns></returns>
        public static ModuleFunctionCollection GetModuleFunctions(this Process process, string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentNullException(nameof(moduleName));

            ProcessModule module = process.GetModule(moduleName);

            if (module is null) throw new DllNotFoundException($"Could not find library at given address.");

            return GetModuleFunctions(process, module.BaseAddress);
        }

        /// <summary>
        /// Проверяет, активно ли главное окно процесса
        /// </summary>
        /// <param name="process">Процесс, у которого проверяется окно</param>
        /// <returns></returns>
        public static bool IsActiveWindow(this Process process) => process.MainWindowHandle == User32.GetForegroundWindow();
    }
}