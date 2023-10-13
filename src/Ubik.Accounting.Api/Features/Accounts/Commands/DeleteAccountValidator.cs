using FluentValidation;
using static Ubik.Accounting.Api.Features.Accounts.Commands.DeleteAccount;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
    {
        public DeleteAccountValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("Id is required");
        }
    }
}
