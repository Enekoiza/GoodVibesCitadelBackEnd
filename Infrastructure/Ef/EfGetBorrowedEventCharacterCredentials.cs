namespace Infrastructure.Ef;

using ApplicationService;
using Domain;
using Domain.Dto;
using Domain.Entities;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class EfGetBorrowedEventCharacterCredentials(
    AppDbContext db,
    ICharacterPasswordProtector passwordProtector) : IGetBorrowedEventCharacterCredentials
{
    public async Task<ErrorOr<BorrowedCharacterCredentialsResponseDto>> GetCredentials(
        string eventId,
        string assignedUsername)
    {
        var eventEntity = await this.FindEventAsync(eventId);
        if (eventEntity is null)
        {
            return Error.NotFound(description: "Evento no encontrado.");
        }

        if (!EventCredentialEventTypes.RevealsBorrowedCharacterCredentials(eventEntity.EventType))
        {
            return this.NoneResponse();
        }

        var borrowedCharacters = await this.ResolveBorrowedCharactersAsync(eventEntity, assignedUsername);
        if (borrowedCharacters.Count == 0)
        {
            return this.NoneResponse();
        }

        if (EventCredentialEventTypes.IsAfterBorrowedCredentialsWindow(eventEntity.EventTime))
        {
            return this.ExpiredResponse();
        }

        var characterSummaries = borrowedCharacters
            .Select(character => new BorrowedCharacterCredentialsDto(
                character.Name,
                character.User.UserName ?? string.Empty,
                string.Empty,
                false))
            .ToList();

        if (EventCredentialEventTypes.IsBeforeBorrowedCredentialsWindow(eventEntity.EventTime))
        {
            return new BorrowedCharacterCredentialsResponseDto(
                BorrowedCharacterCredentialsVisibility.Scheduled,
                characterSummaries);
        }

        return new BorrowedCharacterCredentialsResponseDto(
            BorrowedCharacterCredentialsVisibility.Available,
            borrowedCharacters
                .Select(character => new BorrowedCharacterCredentialsDto(
                    character.Name,
                    character.User.UserName ?? string.Empty,
                    character.Login ?? string.Empty,
                    !string.IsNullOrEmpty(character.Password)))
                .ToList());
    }

    public async Task<ErrorOr<string>> GetPassword(
        string eventId,
        string assignedUsername,
        string characterName)
    {
        if (string.IsNullOrWhiteSpace(characterName))
        {
            return Error.Validation(description: "El nombre del personaje es obligatorio.");
        }

        var eventEntity = await this.FindEventAsync(eventId);
        if (eventEntity is null)
        {
            return Error.NotFound(description: "Evento no encontrado.");
        }

        if (!EventCredentialEventTypes.RevealsBorrowedCharacterCredentials(eventEntity.EventType))
        {
            return Error.Forbidden(description: "Este tipo de evento no permite consultar credenciales.");
        }

        if (!EventCredentialEventTypes.IsWithinBorrowedCredentialsWindow(eventEntity.EventTime))
        {
            return Error.Forbidden(description: "Las credenciales aún no están disponibles para este evento.");
        }

        var borrowedCharacters = await this.ResolveBorrowedCharactersAsync(eventEntity, assignedUsername);
        var character = borrowedCharacters.SingleOrDefault(c =>
            string.Equals(c.Name, characterName, StringComparison.Ordinal));

        if (character is null)
        {
            return Error.NotFound(description: "No tienes acceso a las credenciales de este personaje.");
        }

        if (string.IsNullOrEmpty(character.Password))
        {
            return string.Empty;
        }

        if (!passwordProtector.TryReveal(character.Password, out var plainPassword))
        {
            return Error.Conflict(
                description: "La contraseña no se puede mostrar. Pide al dueño que la actualice en sus credenciales.");
        }

        return plainPassword;
    }

    private BorrowedCharacterCredentialsResponseDto NoneResponse() =>
        new(BorrowedCharacterCredentialsVisibility.None, []);

    private BorrowedCharacterCredentialsResponseDto ExpiredResponse() =>
        new(BorrowedCharacterCredentialsVisibility.Expired, []);

    private async Task<Event?> FindEventAsync(string eventId)
    {
        if (!int.TryParse(eventId, out var parsedEventId))
        {
            return null;
        }

        return await db.Events.SingleOrDefaultAsync(e => e.Id == parsedEventId);
    }

    private async Task<List<Character>> ResolveBorrowedCharactersAsync(Event eventEntity, string assignedUsername)
    {
        if (string.IsNullOrWhiteSpace(assignedUsername) || eventEntity.PartyComposition is null)
        {
            return [];
        }

        var assignedCharacterNames = eventEntity.PartyComposition
            .SelectMany(composition => composition.Slots)
            .Where(slot => string.Equals(slot.Username, assignedUsername, StringComparison.OrdinalIgnoreCase))
            .Select(slot => slot.CharacterName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (assignedCharacterNames.Count == 0)
        {
            return [];
        }

        var characters = await db.Characters
            .Include(character => character.User)
            .Where(character => assignedCharacterNames.Contains(character.Name))
            .ToListAsync();

        return characters
            .Where(character =>
                !string.Equals(character.User.UserName, assignedUsername, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
