using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccount
    {
        public class AddGameCommand : IRequest<AccountResult>
        {
            public string Name { get; set; }
            public string Publisher { get; set; }
            public int ConsoleId { get; set; }
        }

        public class AccountResult
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Publisher { get; set; }
            public int ConsoleId { get; set; }
        }
    }
}
