using System.Collections.Generic;
using System.Diagnostics;
using System.WinApi;

namespace System.MemoryInteraction
{
    public unsafe class MemoryManager : ModuleManager, IMemory
    {
        #region Private variables
        private Allocator m_Allocator;
        private Executor m_Executor;
        private PageManager m_PageManager;
        private PatternManager m_PatternManager;

        private readonly List<ModuleManager> m_ProcessModules;
        #endregion

        #region Initialization
        public MemoryManager(Process process) : base(process, null)
        {
            m_ProcessModules = new List<ModuleManager>();
        }
        #endregion

        #region Indexer
        public ModuleManager this[string moduleName]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Find(X => X.ModuleName == moduleName);

                if(selectedModule is null)
                {
                    ModuleManager newModule = new ModuleManager(m_Process, moduleName);

                    m_ProcessModules.Add(newModule);

                    return m_ProcessModules[m_ProcessModules.Count - 1];
                }

                return selectedModule;
            }
        }

        public ModuleManager this[IntPtr modulePtr]
        {
            get
            {
                ModuleManager selectedModule = m_ProcessModules.Find(X => X.ModulePtr == modulePtr);

                if (selectedModule is null)
                {
                    ModuleManager newModule = new ModuleManager(m_Process, modulePtr);

                    m_ProcessModules.Add(newModule);

                    return m_ProcessModules[m_ProcessModules.Count - 1];
                }

                return selectedModule;
            }
        }
        #endregion

        public virtual bool BlockCopy<TArray>(TArray[] src, int srcIndex, IntPtr dst, int dstOffset, IntPtr count) where TArray : unmanaged
        {
            if (count == IntPtr.Zero) return false;

            if (srcIndex >= src.Length) throw new IndexOutOfRangeException("index is more than the length of the array");

            if (count.ToInt64() > src.Length - srcIndex) throw new IndexOutOfRangeException("count is more than the length of the array");

            fixed (TArray* srcPtr = &src[srcIndex])
            {
                return Kernel32.WriteProcessMemory(m_Process.Handle, dst + dstOffset, (IntPtr)srcPtr, (IntPtr)(count.ToInt64() * sizeof(TArray)), IntPtr.Zero);
            }
        }

        public virtual bool MemoryCopy(IntPtr src, int srcOffset, IntPtr dst, int dstOffset, IntPtr count)
        {
            if (count == IntPtr.Zero) return false;

            return Kernel32.WriteProcessMemory(m_Process.Handle, dst + dstOffset, src + srcOffset, count, IntPtr.Zero);
        }

        public IAllocator GetAllocator()
        {
            if (m_Allocator == null) m_Allocator = new Allocator(m_Process);

            return m_Allocator;
        }

        public Executor GetExecutor()
        {
            if (m_Executor == null) m_Executor = new Executor(m_Process, this);

            return m_Executor;
        }

        public PageManager GetPageManager()
        {
            if (m_PageManager == null) m_PageManager = new PageManager(m_Process);

            return m_PageManager;
        }

        public PatternManager GetPatternManager()
        {
            if (m_PatternManager == null) m_PatternManager = new PatternManager(m_Process, this);

            return m_PatternManager;
        }

        public static MemoryManager GetCurrentProcessMemory() => new MemoryManager(Process.GetCurrentProcess());
    }
}