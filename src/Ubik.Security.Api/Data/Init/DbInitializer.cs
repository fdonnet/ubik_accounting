namespace Ubik.Security.Api.Data.Init
{
    internal class DbInitializer
    {
        internal async Task InitializeAsync(SecurityDbContext context)
        {
            await UsersData.LoadAsync(context);
            await AuthorizationsData.LoadAsync(context);
            await RolesData.LoadAsync(context);
            await RolesAuthorizationsData.LoadAsync(context);
            await TenantsData.LoadAsync(context);
        }
    }
}
