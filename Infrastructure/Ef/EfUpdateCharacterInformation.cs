namespace Infrastructure.Ef;

using Domain.Entities;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;

public class EfUpdateCharacterInformation : IUpdatedCharacterInformation
{
    private readonly AppDbContext db;
    private readonly ICharacterPasswordProtector passwordProtector;

    public EfUpdateCharacterInformation(AppDbContext db, ICharacterPasswordProtector passwordProtector)
    {
        this.db = db;
        this.passwordProtector = passwordProtector;
    }

    public async Task<ErrorOr<Success>> Process(string userId, List<CharacterInfoUpdate> characterInfos)
    {
        try
        {
            var duplicateNameInRequest = characterInfos
                .GroupBy(ci => ci.Name, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateNameInRequest is not null)
            {
                return Error.Conflict(
                    CharacterNameConflict.Code,
                    CharacterNameConflict.Message("otro personaje de la misma lista"));
            }

            var characterInfoNames = characterInfos.Select(ci => ci.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var charactersToDelete = this.db.Characters
                .Where(c => c.Userid == userId && !characterInfoNames.Contains(c.Name));

            this.db.Characters.RemoveRange(charactersToDelete);

            foreach (var characterInfo in characterInfos)
            {
                var classId = this.db.Classes
                    .Single(c => c.Name == characterInfo.ClassName).Id;

                var character = this.db.Characters
                    .SingleOrDefault(c => c.Userid == userId && c.Name == characterInfo.Name);

                var login = characterInfo.Login.Trim();

                if (character is null)
                {
                    var ownerUsername = await this.db.Characters
                        .AsNoTracking()
                        .Where(c => c.Name == characterInfo.Name && c.Userid != userId)
                        .Select(c => c.User!.UserName)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrWhiteSpace(ownerUsername))
                    {
                        return Error.Conflict(
                            CharacterNameConflict.Code,
                            CharacterNameConflict.Message(ownerUsername));
                    }

                    var newCharacter = new Character
                    {
                        Name = characterInfo.Name,
                        Classid = classId,
                        Userid = userId,
                        Level = characterInfo.Level,
                        Login = login,
                        Password = ResolvePasswordForNew(characterInfo, login),
                    };

                    if (!string.IsNullOrEmpty(newCharacter.Password) && string.IsNullOrEmpty(login))
                    {
                        return Error.Validation("Character.Password", "Password requires a login.");
                    }

                    this.db.Characters.Add(newCharacter);
                }
                else
                {
                    character.Classid = classId;
                    character.Level = characterInfo.Level;
                    character.Login = login;
                    character.Password = ResolvePasswordForUpdate(character, characterInfo, login);
                }
            }

            await this.db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Error.Conflict(
                CharacterNameConflict.Code,
                "El nombre de personaje ya está registrado por otro usuario.");
        }
        catch (Exception)
        {
            return Error.Unexpected("Exception Thrown");
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> UpdateCredentials(
        string userId,
        string characterName,
        string login,
        string? password)
    {
        try
        {
            var character = await this.db.Characters
                .SingleOrDefaultAsync(c => c.Userid == userId && c.Name == characterName);

            if (character is null)
            {
                return Error.NotFound("Character.NotFound", "Character not found.");
            }

            var trimmedLogin = login.Trim();
            character.Login = trimmedLogin;

            if (password is not null)
            {
                if (password.Length == 0)
                {
                    character.Password = string.Empty;
                }
                else if (string.IsNullOrEmpty(trimmedLogin))
                {
                    return Error.Validation("Character.Password", "Password requires a login.");
                }
                else
                {
                    character.Password = this.passwordProtector.Protect(password);
                }
            }

            await this.db.SaveChangesAsync();
        }
        catch (Exception)
        {
            return Error.Unexpected("Exception Thrown");
        }

        return Result.Success;
    }

    private string ResolvePasswordForNew(CharacterInfoUpdate characterInfo, string login)
    {
        if (characterInfo.Password is null)
        {
            return string.Empty;
        }

        if (characterInfo.Password.Length == 0)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(login))
        {
            return string.Empty;
        }

        return this.passwordProtector.Protect(characterInfo.Password);
    }

    private string ResolvePasswordForUpdate(Character character, CharacterInfoUpdate characterInfo, string login)
    {
        if (string.IsNullOrEmpty(login))
        {
            return string.Empty;
        }

        if (characterInfo.Password is null)
        {
            return character.Password;
        }

        if (characterInfo.Password.Length == 0)
        {
            return string.Empty;
        }

        return this.passwordProtector.Protect(characterInfo.Password);
    }
}
