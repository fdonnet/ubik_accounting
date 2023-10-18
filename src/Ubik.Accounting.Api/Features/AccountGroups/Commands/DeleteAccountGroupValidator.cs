using FluentValidation;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.DeleteAccountGroup;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class DeleteAccountGroupValidator : AbstractValidator<DeleteAccountGroupCommand>
    {
        public DeleteAccountGroupValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("Id is required");
        }
    }
}
