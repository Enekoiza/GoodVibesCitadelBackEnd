namespace Infrastructure.Ef;

public interface ICharacterPasswordProtector
{
    string Protect(string plainPassword);

    bool TryUnprotect(string storedPassword, out string plainPassword);

    bool TryReveal(string storedPassword, out string plainPassword);
}
