using FluentValidation;
using SuperHeroAPIs.DTOs;

namespace SuperHeroAPIs.Validators
{
    public class SuperHeroUpdateDtoValidator : AbstractValidator<SuperHeroUpdateDto>
    {
        public SuperHeroUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Place)
                .NotEmpty().WithMessage("Place is required.")
                .MaximumLength(100).WithMessage("Place must not exceed 100 characters.");
        }
    }
}
