using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Mappers
{
    public static class ClassificationMappers
    {
        public static IEnumerable<ClassificationStandardResult> ToClassificationStandardResults(this IEnumerable<Classification> current)
        {
            return current.Select(x => new ClassificationStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version
            });
        }

        public static ClassificationStandardResult ToClassificationStandardResult(this Classification current)
        {
            return new ClassificationStandardResult()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version
            };
        }

        public static ClassificationStatusResult ToClassificationStatusResult(this ClassificationStatus current)
        {
            return new ClassificationStatusResult()
            {
                Id = current.Id,
                IsReady = current.IsReady,
                MissingAccounts = current.MissingAccounts.ToAccountStandardResults()
            };
        }

        public static Classification ToClassification(this AddClassificationCommand addClassificationCommand)
        {
            return new Classification()
            {
                Id = NewId.NextGuid(),
                Code = addClassificationCommand.Code,
                Label = addClassificationCommand.Label,
                Description = addClassificationCommand.Description
            };
        }
    }
}
