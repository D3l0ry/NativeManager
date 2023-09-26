using System.MemoryInteractions;

namespace System.Diagnostics
{
    public class ProcessProvider
    {
        private readonly Process _process;

        private readonly Lazy<Allocator> _allocator;
        private readonly Lazy<Executor> _executor;
        private readonly Lazy<SimpleMemoryManager> _simpleMemoryManager;
        private readonly Lazy<MemoryManager> _memoryManager;
        private readonly Lazy<PageManager> _pageManager;

        public ProcessProvider(Process process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));

            _allocator = new Lazy<Allocator>(() => new Allocator(process));
            _executor = new Lazy<Executor>(() => new Executor(_process));
            _simpleMemoryManager = new Lazy<SimpleMemoryManager>(() => new SimpleMemoryManager(_process));
            _memoryManager = new Lazy<MemoryManager>(() => new MemoryManager(_process));
            _pageManager = new Lazy<PageManager>(() => new PageManager(_process));
        }

        public Allocator Allocator => _allocator.Value;

        public Executor Executor => _executor.Value;

        public PageManager PageManager => _pageManager.Value;

        public SimpleMemoryManager SimpleMemoryManager => _simpleMemoryManager.Value;

        public MemoryManager MemoryManager => _memoryManager.Value;
    }
}