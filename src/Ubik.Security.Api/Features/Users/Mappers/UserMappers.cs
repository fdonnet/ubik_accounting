using MassTransit;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Events;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Mappers
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

        public static AddUserResult ToAddUserResult(this User current)
        {
            return new AddUserResult()
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
