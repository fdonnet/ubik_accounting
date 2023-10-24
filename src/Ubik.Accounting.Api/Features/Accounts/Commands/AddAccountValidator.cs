using FluentValidation;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccountValidator : AbstractValidator<AddAccountCommand>
    {
        public AddAccountValidator()
        {
            RuleFor(command => command.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(20).WithMessage("Code must be 20 characters max.");

            RuleFor(command => command.Label)
                .NotEmpty().WithMessage("Label is required")
                .MaximumLength(100).WithMessage("Label must be 100 characters max.");

            RuleFor(command => command.Description)
                .MaximumLength(700).WithMessage("Description must be 700 characters max.");

            RuleFor(command => command.CurrencyId)
                .NotEmpty().WithMessage("CurrencyId is required");
        }
    }
}
