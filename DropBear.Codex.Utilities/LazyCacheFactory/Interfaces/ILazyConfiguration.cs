using DropBear.Codex.Utilities.LazyCacheFactory.Models;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;

public interface ILazyConfiguration<T>
{
    ILazyConfiguration<T> WithInitialization(Func<T> initializer);
    ILazyConfiguration<T> WithAsyncInitialization(Func<Task<T>> asyncInitializer);
    ILazyConfiguration<T> WithCaching(TimeSpan expiration = default);
    Lazy<T> Build();
    AsyncLazy<T> BuildAsync();
    ResettableLazy<T> BuildResettable();
    ResettableLazy<T> BuildResettableAsync();
    Lazy<T> BuildWithForcedSync();
    ResettableLazy<T> BuildWithForcedSyncResettable();
    AsyncResettableLazy<T> BuildAsyncResettable();
    AsyncResettableLazy<T> BuildWithForcedSyncAsyncResettable();
}
