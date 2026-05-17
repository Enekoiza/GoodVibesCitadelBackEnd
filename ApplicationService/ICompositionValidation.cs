namespace ApplicationService;

using Domain.Models;

public interface ICompositionValidation
{
    public bool Validate(CompositionValidationModel composition);
}