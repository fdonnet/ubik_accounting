using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Extensions;
using Ubik.Security.Api.Data.Config;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Data
{
    public class SecurityDbContext(DbContextOptions<SecurityDbContext> options
        , ICurrentUserService userService) : DbContext(options)
    {
        private readonly ICurrentUserService _currentUserService = userService;
        private readonly Guid _tenantId = userService.CurrentUser.TenantIds[0];

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserTenant> UsersTenants { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<RoleAuthorization> RolesAuthorizations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSnakeCaseNamingConvention();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var err = new CustomError()
                {
                    ErrorCode = "DB_CONCURRENCY_CONFLICT",
                    ErrorFriendlyMessage = "You don't have the last version or the ressource, refresh your data before updating.",
                    ErrorValueDetails = "Version",
                };
                var conflict = new UpdateDbConcurrencyException()
                {
                    CustomErrors = [err]
                };

                throw conflict;
            }
        }

        public void SetAuditAndSpecialFields()
        {
            ChangeTracker.SetSpecialFields(_currentUserService);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Build for Masstransit inbox/outbox
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            //TenantId
            //SetTenantId(modelBuilder);

            //Configure
            new TenantConfiguration().Configure(modelBuilder.Entity<Tenant>());
            new UserConfiguration().Configure(modelBuilder.Entity<User>());
            new UserTenantConfiguration().Configure(modelBuilder.Entity<UserTenant>());
            new RoleConfiguration().Configure(modelBuilder.Entity<Role>());
            new AuthorizationConfiguration().Configure(modelBuilder.Entity<Authorization>());
            new RoleAuthorizationConfiguration().Configure(modelBuilder.Entity<RoleAuthorization>());

            base.OnModelCreating(modelBuilder);
        }

        //private void SetTenantId(ModelBuilder modelBuilder)
        //{
            
        //}
    }
}
