namespace Infrastructure.Ef;

public static class CharacterNameConflict
{
    public const string Code = "Character.Name";

    public static string Message(string ownerUsername) =>
        $"El nombre de personaje ya lo tiene registrado {ownerUsername}";
}
