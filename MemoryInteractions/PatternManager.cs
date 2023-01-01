using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace System.MemoryInteractions
{
    /// <summary>
    /// Предоставляет функции для получения адреса по определенному паттерну
    /// </summary>
    public unsafe class PatternManager
    {
        private readonly Process _Process;
        private readonly MemoryManager _Memory;

        public PatternManager(Process process) : this(process, process?.GetMemoryManager()) { }

        public PatternManager(Process process, MemoryManager memory)
        {
            ProcessExtensions.CheckProcess(process);

            if (memory == null)
            {
                throw new ArgumentNullException(nameof(memory));
            }

            _Process = process;
            _Memory = memory;
        }

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <returns></returns>
        public IntPtr this[string moduleName, string pattern] => FindPattern(moduleName, pattern);

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <param name="moduleName">Имя модуля, в котором искать адрес</param>
        /// <param name="pattern">Паттерн</param>
        /// <param name="offset">Смещение, добавляемое после поиска</param>
        /// <returns></returns>
        public IntPtr FindPattern(string moduleName, byte[] pattern, int offset = 0)
        {
            ProcessModule moduleInfo = _Process.GetModule(moduleName);

            return FindPattern(moduleInfo, pattern, offset);
        }

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <param name="moduleName">Имя модуля, в котором искать адрес</param>
        /// <param name="pattern">Паттерн</param>
        /// <param name="offset">Смещение, добавляемое после поиска</param>
        /// <returns></returns>
        public IntPtr FindPattern(string moduleName, string pattern, int offset = 0) => FindPattern(moduleName, GetPattern(pattern), offset);

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <param name="moduleInfo">Модуль процесса</param>
        /// <param name="pattern">Паттерн</param>
        /// <param name="offset">Смещение, добавляемое после поиска</param>
        /// <returns></returns>
        public IntPtr FindPattern(ProcessModule moduleInfo, byte[] pattern, int offset = 0)
        {
            if (moduleInfo == null)
            {
                throw new ArgumentNullException(nameof(moduleInfo));
            }

            return FindPattern(moduleInfo.BaseAddress, (IntPtr)moduleInfo.ModuleMemorySize, pattern, offset);
        }

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <param name="moduleInfo">Модуль процесса</param>
        /// <param name="pattern">Паттерн</param>
        /// <param name="offset">Смещение, добавляемое после поиска</param>
        /// <returns></returns>
        public IntPtr FindPattern(ProcessModule moduleInfo, string pattern, int offset = 0) => FindPattern(moduleInfo, GetPattern(pattern), offset);

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <param name="startAddress">Стартовый адрес поиска</param>
        /// <param name="endAddress">Конечный адрес поиска</param>
        /// <param name="pattern">Паттерн</param>
        /// <param name="offset">Смещение, добавляемое после поиска</param>
        /// <returns></returns>
        public virtual IntPtr FindPattern(IntPtr startAddress, IntPtr endAddress, byte[] pattern, int offset)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            long indexMax = startAddress.ToInt64() + endAddress.ToInt64();

            for (long pIndex = startAddress.ToInt64(); pIndex < indexMax; pIndex++)
            {
                bool Found = true;

                for (int MIndex = 0; MIndex < pattern.Length; MIndex++)
                {
                    Found = pattern[MIndex] == 0 || _Memory.Read<byte>((IntPtr)(pIndex + MIndex)) == pattern[MIndex];

                    if (!Found)
                    {
                        break;
                    }
                }

                if (Found)
                {
                    return (IntPtr)(pIndex + offset);
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Получает адрес по определенному паттерну
        /// </summary>
        /// <param name="startAddress">Стартовый адрес поиска</param>
        /// <param name="endAddress">Конечный адрес поиска</param>
        /// <param name="pattern">Паттерн</param>
        /// <param name="offset">Смещение, добавляемое после поиска</param>
        /// <returns></returns>
        public IntPtr FindPattern(IntPtr startAddress, IntPtr endAddress, string pattern, int offset) => FindPattern(startAddress, endAddress, GetPattern(pattern), offset);

        /// <summary>
        /// Получает массив байт из текстового паттерна (Пример: FF 91 01 ?? F5)
        /// </summary>
        /// <param name="pattern">Паттерн, который нужно преобразовать в массив байт</param>
        /// <returns></returns>
        private byte[] GetPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            Regex regex = new Regex(@"[a-fA-F0-9]{2}|[?]{2}");
            MatchCollection matches = regex.Matches(pattern);
            List<byte> patternsBytes = new List<byte>(matches.Count);

            foreach (Match currentMatch in matches)
            {
                string matchValue = currentMatch.Value;

                if (matchValue == "??")
                {
                    patternsBytes.Add(0);

                    continue;
                }

                byte matchValueToByte = Convert.ToByte(matchValue, 16);

                patternsBytes.Add(matchValueToByte);
            }

            return patternsBytes.ToArray();
        }
    }
}