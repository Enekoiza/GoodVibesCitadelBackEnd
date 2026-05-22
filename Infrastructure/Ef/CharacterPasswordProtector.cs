namespace Infrastructure.Ef;

using Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

public class CharacterPasswordProtector : ICharacterPasswordProtector
{
    private readonly IDataProtector protector;

    public CharacterPasswordProtector(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<CharacterPasswordProtectionOptions> options)
    {
        var purpose = options.Value.Purpose;
        if (string.IsNullOrWhiteSpace(purpose))
        {
            throw new InvalidOperationException(
                $"Configuration '{CharacterPasswordProtectionOptions.SectionName}:Purpose' is required.");
        }

        this.protector = dataProtectionProvider.CreateProtector(purpose);
    }

    public string Protect(string plainPassword) => this.protector.Protect(plainPassword);

    public bool TryUnprotect(string storedPassword, out string plainPassword)
    {
        try
        {
            plainPassword = this.protector.Unprotect(storedPassword);
            return true;
        }
        catch
        {
            plainPassword = string.Empty;
            return false;
        }
    }

    public bool TryReveal(string storedPassword, out string plainPassword)
    {
        plainPassword = string.Empty;

        if (string.IsNullOrEmpty(storedPassword))
        {
            return true;
        }

        if (this.TryUnprotect(storedPassword, out plainPassword))
        {
            return true;
        }

        if (IsIdentityPasswordHash(storedPassword))
        {
            return false;
        }

        // Legacy plaintext stored before encryption was introduced.
        plainPassword = storedPassword;
        return true;
    }

    private static bool IsIdentityPasswordHash(string storedPassword) =>
        storedPassword.Length >= 60 && storedPassword.StartsWith("AQAAAA", StringComparison.Ordinal);
}
