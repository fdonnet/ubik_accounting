using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Test
{
    //TODO: need to be more portable
    public class AccountingTestDbFixture
    {
        private const string ConnectionString = @"server=localhost;port=3306;database=ubik_accounting_test;uid=root;password=godjade";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public AccountingTestDbFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        var initDb = new DbInitializer();
                        initDb.Initialize(context);
                    }
                    _databaseInitialized = true;
                }
            }
        }

        public static AccountingContext CreateContext()
            => new(
                new DbContextOptionsBuilder<AccountingContext>()
                    .UseMySql(ConnectionString, new MariaDbServerVersion(new Version(11, 1, 2)))
                    .Options,new CurrentUserService());
    }
}
