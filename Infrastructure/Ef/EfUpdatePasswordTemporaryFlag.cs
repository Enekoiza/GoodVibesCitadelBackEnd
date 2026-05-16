namespace Infrastructure.Ef;

using Microsoft.EntityFrameworkCore;

public class EfUpdatePasswordTemporaryFlag : IUpdatePasswordTemporaryFlag
{
    private readonly AppDbContext db;

    public EfUpdatePasswordTemporaryFlag(AppDbContext db)
    {
        this.db = db;
    }

    public async Task Process(string userId)
    {
        var user = await this.db.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return;
        }
        
        user.IsPasswordTemporary = false;
        
        await this.db.SaveChangesAsync();
    }
}