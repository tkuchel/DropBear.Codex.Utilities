using DropBear.Codex.Utilities.LazyCacheFactory.Models;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;

public interface ILazyConfiguration<T>
{
    ILazyConfiguration<T> WithInitialization(Func<T> initializer);
    ILazyConfiguration<T> WithCaching(TimeSpan expiration = default);
    Lazy<T> Build();
    ResettableLazy<T> BuildResettable();
    AsyncResettableLazy<T> BuildAsyncResettable();
    ILazyConfiguration<T> WithAsyncInitialization(Func<CancellationToken, Task<T>> asyncInitializer);
    AsyncLazy<T> BuildAsync(CancellationToken cancellationToken = default);
}
