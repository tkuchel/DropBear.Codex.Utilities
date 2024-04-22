namespace DropBear.Codex.Utilities.Hashing.Interfaces;

public interface IHashFactory
{
    IHasher GetHasher(string key);
}
