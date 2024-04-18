namespace DropBear.Codex.Utilities.Hashing.Interfaces;

public interface IHashingServiceFactory
{
    IHashingService CreateService(string key);
}
