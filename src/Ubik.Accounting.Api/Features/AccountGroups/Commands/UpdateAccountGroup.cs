using MediatR;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class UpdateAccountGroup
    {
        //Input
        public record UpdateAccountGroupCommand : IRequest<UpdateAccountGroupResult>
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            public Guid Version { get; set; }
        }

        //Output
        public record UpdateAccountGroupResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            public Guid Version { get; set; }
        }
    }
}
