namespace Infrastructure.Ef;

using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class EfGetAllPartyCompositions : IGetAllPartyCompositions
{
    private readonly AppDbContext db;

    public EfGetAllPartyCompositions(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<CompositionModel>> Process()
    {
        var allCompositions = await this.db.Compositions.ToListAsync();
        
        return allCompositions.Select(MapToCompositionModel).ToList();
    }

    private static CompositionModel MapToCompositionModel(Composition composition)
    {
        return new(
            composition.Name, 
            composition.DpsCount, 
            composition.BishopCount, 
            composition.BardCount,
            composition.BufferCount, 
            composition.TankCount, 
            composition.RechargerCount, 
            composition.IsPartyFull);
    }
}