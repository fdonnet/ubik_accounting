using MassTransit;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Results;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Events;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Mappers
{
    public static class UserMappers
    {
        public static User ToUser(this AddUserCommand current)
        {
            return new User()
            {
                Id = NewId.NextGuid(),
                Firstname = current.Firstname,
                Lastname = current.Lastname,
                Email = current.Email,
            };
        }

        public static UserStandardResult ToUserStandardResult(this User current)
        {
            return new UserStandardResult()
            {
                Id = current.Id,
                Firstname = current.Firstname,
                Lastname = current.Lastname,
                Email = current.Email,
                IsActivated = current.IsActivated,
                Version = current.Version,
            };
        }

        public static UserAdminResult ToUserAdminResult(this User current, Dictionary<Guid, List<AuthorizationStandardResult>> authorizationsByTenant)
        {
            return new UserAdminResult()
            {
                Id = current.Id,
                Firstname = current.Firstname,
                Lastname = current.Lastname,
                Email = current.Email,
                IsActivated = current.IsActivated,
                IsMegaAdmin = current.IsMegaAdmin,
                SelectedTenantId = current.SelectedTenantId,
                Version = current.Version,
                AuthorizationsByTenantIds = authorizationsByTenant
            };
        }

        public static UserAdded ToUserAdded(this User current)
        {
            return new UserAdded()
            {
                Id = current.Id,
                Firstname = current.Firstname,
                Lastname = current.Lastname,
                Email = current.Email,
                Version = current.Version,
            };
        }
    }
}
