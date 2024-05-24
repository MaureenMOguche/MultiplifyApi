using FluentValidation;
using Multiplify.Application.Dtos.User;

namespace Multiplify.Application.DtoValidations;
public class EntreprenuerCompleteRegDto : AbstractValidator<EntreprenuerCompleteRegistration>
{
    public EntreprenuerCompleteRegDto()
    {
        RuleFor(x => x.BusinessStage).IsInEnum().WithMessage("Invalid business stage").NotEmpty().WithMessage("Business stage is required");
    }
}
