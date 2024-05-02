using DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

public static class LazyFactory
{
    public static ILazyConfiguration<T> Create<T>(MemoryCacheManager cacheManager) => new LazyBuilder<T>(cacheManager);
}
