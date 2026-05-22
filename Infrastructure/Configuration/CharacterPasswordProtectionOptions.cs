namespace Infrastructure.Configuration;

public class CharacterPasswordProtectionOptions
{
    public const string SectionName = "CharacterPasswordProtection";

    public string Purpose { get; set; } = "GoodVibes.Character.Password.v1";
}
