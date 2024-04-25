namespace DropBear.Codex.Utilities.Hashing.Interfaces;

public interface IHashBuilder
{
    IHasher GetHasher(string key);
}
