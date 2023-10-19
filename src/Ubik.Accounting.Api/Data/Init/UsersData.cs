using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class UsersData
    {
        internal static void Load(AccountingContext context)
        {
            var userId2 = Guid.NewGuid();

            if (!context.Users.Any())
            {
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForTenants = new BaseValuesForTenants();

                var users = new User[]
                            {
                new User
                {
                    Id = baseValuesForUsers.UserId1,
                    Name = "testuser",
                    Email = "test@gmail.com",
                    TenantId = baseValuesForTenants.TenantId
                },
                new User
                {
                    Id = userId2,
                    Name = "test01",
                    Email = "test01@gmail.com",
                    TenantId = baseValuesForTenants.TenantId
                },
                            };

                foreach (User u in users)
                {
                    context.Users.Add(u);
                }
                context.SaveChanges();
            }
        }
    }
}
