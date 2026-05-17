namespace ApplicationService;

using Domain;
using Domain.Models;

public class CompositionValidation : ICompositionValidation
{
    public bool Validate(CompositionValidationModel composition)
    {
        if (composition.PartyType == PartyType.Support && composition is { RechargerCount: > 0 })
        {
            return true;
        }
        
        if (composition.PartyType is PartyType.Farm or PartyType.Raid)
        {
            return true;
        }
        
        if (composition is { IsPartyFull: true, BishopCount: > 0, RechargerCount: > 0 })
        {
            return true;
        }

        return false;
    }
}