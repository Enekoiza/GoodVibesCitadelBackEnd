namespace Infrastructure.Ef;

using Microsoft.EntityFrameworkCore;
using Shared.DTO;

public class EfGetAllCharacters : IGetAllCharacters
{
    private readonly AppDbContext db;

    public EfGetAllCharacters(AppDbContext db)
    {
        this.db = db;
    }

    public IReadOnlyDictionary<string, List<CharacterInfoResponse>> Process()
    {
        return this.db.Characters
            .Include(c => c.User)
            .Include(c => c.Class)
            .GroupBy(c => c.User.UserName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(c => new CharacterInfoResponse(
                    c.Name,
                    c.Class.Name,
                    c.Class.Type,
                    c.Level,
                    c.Login,
                    !string.IsNullOrEmpty(c.Password))).ToList()
            );
    }
}