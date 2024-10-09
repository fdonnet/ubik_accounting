using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Events;

namespace Ubik.Security.Api.Features.Tenants.Mappers
{
    public static class TenantMappers
    {
        public static IEnumerable<TenantStandardResult> ToTenantStandardResults(this IEnumerable<Tenant> current)
        {
            return current.Select(x => new TenantStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                IsActivated = x.IsActivated,
                Description = x.Description,
                Version = x.Version,
            });
        }

        public static Tenant ToTenant(this AddTenantCommand current)
        {
            return new Tenant
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
            };
        }

        public static Tenant ToTenant(this UpdateTenantCommand current)
        {
            return new Tenant
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
            };
        }

        public static TenantAdded ToTenantAdded(this Tenant current)
        {
            return new TenantAdded()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
            };
        }

        public static TenantUpdated ToTenantUpdated(this Tenant current)
        {
            return new TenantUpdated()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
            };
        }

        public static Tenant ToTenant(this Tenant forUpd, Tenant model)
        {
            model.Id = forUpd.Id;
            model.Code = forUpd.Code;
            model.Label = forUpd.Label;
            model.Description = forUpd.Description;
            model.IsActivated = forUpd.IsActivated;
            model.Version = forUpd.Version;

            return model;
        }

        public static TenantStandardResult ToTenantStandardResult(this Tenant current)
        {
            return new TenantStandardResult()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                IsActivated = current.IsActivated,
                Version = current.Version,
            };
        }
    }
}
