using FluentValidation;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(command => command.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(20).WithMessage("Code must be 20 characters max.");

            RuleFor(command => command.Label)
                .NotEmpty().WithMessage("Label is required")
                .MaximumLength(100).WithMessage("Label must be 100 characters max.");

            RuleFor(command => command.Category)
                .NotEmpty().WithMessage("Category is required");

            RuleFor(command => command.Domain)
                .NotEmpty().WithMessage("Domain is required");

            RuleFor(command => command.Description)
                .MaximumLength(700).WithMessage("Description must be 700 characters max.");

            RuleFor(command => command.Version)
                .NotEmpty().WithMessage("Version is required. Need to be checked for concurrency updates.");

            RuleFor(command => command.CurrencyId)
                .NotEmpty().WithMessage("CurrencyId is required.");
        }
    }
}
