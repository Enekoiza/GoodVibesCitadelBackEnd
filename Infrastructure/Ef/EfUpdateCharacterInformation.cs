namespace Infrastructure.Ef;

using ErrorOr;
using Shared.DTO;

public class EfUpdateCharacterInformation : IUpdatedCharacterInformation
{
    private readonly AppDbContext db;

    public EfUpdateCharacterInformation(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<ErrorOr<Success>> Process(string userId, List<CharacterInfo> characterInfos)
    {
        try
        {
            var characterInfoNames = characterInfos.Select(ci => ci.Name).ToHashSet();

            var charactersToDelete = this.db.Characters
                .Where(c => c.Userid == userId && !characterInfoNames.Contains(c.Name));

            this.db.Characters.RemoveRange(charactersToDelete);
            
            foreach (var characterInfo in characterInfos)
            {
                var classId = this.db.Classes
                    .Single(c => c.Name == characterInfo.ClassName).Id;

                var character = this.db.Characters
                    .SingleOrDefault(c => c.Name == characterInfo.Name);

                if (character is null)
                {
                    this.db.Characters.Add(new()
                    {
                        Name = characterInfo.Name,
                        Classid = classId,
                        Userid = userId
                    });
                }
                else if (character.Userid != userId)
                {
                    character.Userid = userId;
                }
            }

            await this.db.SaveChangesAsync();
        }
        catch (Exception)
        {
            Error.Unexpected("Exception Thrown");
        }
        
        return Result.Success;
    }
}