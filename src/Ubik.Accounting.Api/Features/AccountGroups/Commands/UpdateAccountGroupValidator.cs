using FluentValidation;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class UpdateAccountGroupValidator : AbstractValidator<UpdateAccountGroupCommand>
    {
        public UpdateAccountGroupValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(command => command.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(20).WithMessage("Code must be 20 characters max.");

            RuleFor(command => command.Label)
                .NotEmpty().WithMessage("Label is required")
                .MaximumLength(100).WithMessage("Label must be 100 characters max.");

            RuleFor(command => command.Description)
                .MaximumLength(700).WithMessage("Description must be 700 characters max.");

            RuleFor(command => command.Version)
                .NotEmpty().WithMessage("Version is required. Need to be checked for concurrency updates.");
        }
    }
}
