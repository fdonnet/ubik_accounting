using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Roles.Mappers
{
    public static class RoleMappers
    {
        public static Role ToRole(this Role forUpd, Role model)
        {
            model.Id = forUpd.Id;
            model.Code = forUpd.Code;
            model.Label = forUpd.Label;
            model.Description = forUpd.Description;
            model.Version = forUpd.Version;

            return model;
        }
    }
}
