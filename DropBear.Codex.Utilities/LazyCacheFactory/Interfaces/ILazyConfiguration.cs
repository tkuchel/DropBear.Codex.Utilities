namespace DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;

public interface ILazyConfiguration<T>
{
    ILazyConfiguration<T> WithInitialization(Func<T> initializer);
    ILazyConfiguration<T> WithCaching(TimeSpan expiration);
    Lazy<T> Build();
}
