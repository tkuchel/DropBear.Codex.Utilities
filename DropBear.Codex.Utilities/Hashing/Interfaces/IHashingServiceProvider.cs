namespace DropBear.Codex.Utilities.Hashing.Interfaces;

public interface IHashingServiceProvider
{
    IHashingService GetHashingService(string key);
}
