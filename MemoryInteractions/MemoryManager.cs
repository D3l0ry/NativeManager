using System.Collections.Generic;
using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public sealed unsafe class MemoryManager : ModuleManager, IMemory
    {
        #region Private variables
        private Lazy<Allocator> m_Allocator = new Lazy<Allocator>();
        private Lazy<Executor> m_Executor = new Lazy<Executor>();
        private Lazy<PageManager> m_PageManager = new Lazy<PageManager>();
        private Lazy<PatternManager> m_PatternManager = new Lazy<PatternManager>();

        private readonly Lazy<List<ModuleManager>> m_ProcessModules = new Lazy<List<ModuleManager>>();
        #endregion

        #region Initialization
        public MemoryManager(Process process) : base(process, null)
        {
            m_Allocator = new Lazy<Allocator>(() => new Allocator(m_Process));
            m_Executor = new Lazy<Executor>(() => new Executor(m_Process,GetAllocator()));
            m_PageManager = new Lazy<PageManager>(() => new PageManager(m_Process));
            m_PatternManager = new Lazy<PatternManager>(() => new PatternManager(m_Process, this));
            m_ProcessModules = new Lazy<List<ModuleManager>>();
        }
        #endregion

        #region Indexer
        public ModuleManager this[string moduleName]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Value.Find(X => X.ModuleName == moduleName);

                if(selectedModule is null)
                {
                    ModuleManager newModule = new ModuleManager(m_Process, moduleName);

                    m_ProcessModules.Value.Add(newModule);

                    return m_ProcessModules.Value[m_ProcessModules.Value.Count - 1];
                }

                return selectedModule;
            }
        }

        public ModuleManager this[IntPtr modulePtr]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Value.Find(X => X.ModulePtr == modulePtr);

                if (selectedModule is null)
                {
                    ModuleManager newModule = new ModuleManager(m_Process, modulePtr);

                    m_ProcessModules.Value.Add(newModule);

                    return m_ProcessModules.Value[m_ProcessModules.Value.Count - 1];
                }

                return selectedModule;
            }
        }
        #endregion

        public bool BlockCopy<TArray>(TArray[] src, int srcIndex, IntPtr dst, int dstOffset, IntPtr count) where TArray : unmanaged
        {
            if (count == IntPtr.Zero) return false;

            if (srcIndex >= src.Length) throw new IndexOutOfRangeException("index is more than the length of the array");

            if (count.ToInt64() > src.Length - srcIndex) throw new IndexOutOfRangeException("count is more than the length of the array");

            fixed (TArray* srcPtr = &src[srcIndex])
            {
                return Kernel32.WriteProcessMemory(m_Process.Handle, dst + dstOffset, (IntPtr)srcPtr, (IntPtr)(count.ToInt64() * sizeof(TArray)), IntPtr.Zero);
            }
        }

        public bool MemoryCopy(IntPtr src, int srcOffset, IntPtr dst, int dstOffset, IntPtr count)
        {
            if (count == IntPtr.Zero) return false;

            return Kernel32.WriteProcessMemory(m_Process.Handle, dst + dstOffset, src + srcOffset, count, IntPtr.Zero);
        }

        public IAllocator GetAllocator() => m_Allocator.Value;

        public Executor GetExecutor() => m_Executor.Value;

        public PageManager GetPageManager() => m_PageManager.Value;

        public PatternManager GetPatternManager() => m_PatternManager.Value;

        public static MemoryManager GetCurrentProcessMemory() => new MemoryManager(Process.GetCurrentProcess());
    }
}