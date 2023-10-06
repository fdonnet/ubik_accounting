using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Ubik.ApiService.Common.Services;

namespace Ubik.DB.Common.Extensions
{
    public static class ChangeTrackerExtensions
    {
        //TODO: seems to work, need to manage special deletion when we don't remove a record from the table but only update a bool "deleted" field
        //      new interface ISpecialDelete to be dev
        public static void SetSpecialFields(this ChangeTracker changeTracker, ICurrentUserService currentUserService)
        {
            changeTracker.DetectChanges();
            IEnumerable<EntityEntry> entities = changeTracker.Entries();
            var currentUser = currentUserService.GetCurrentUser();

            SetAuditFields(entities, currentUser);
            SetTenantField(entities, currentUser);
            SetConcurrencyField(entities);
        }

        /// <summary>
        /// Set audit trail fields
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="currentUser"></param>
        private static void SetAuditFields(IEnumerable<EntityEntry> entities, ICurrentUser currentUser)
        {
            var auditEntities = entities
                                .Where(t => t.Entity is IAuditEntity &&
                                (
                                    t.State == EntityState.Added || t.State == EntityState.Modified
                                ));

            if (auditEntities.Any())
            {
                DateTime timestamp = DateTime.UtcNow;
                Guid userId = currentUser.Id;

                foreach (EntityEntry entry in auditEntities)
                {
                    IAuditEntity entity = (IAuditEntity)entry.Entity;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.CreatedAt = timestamp;
                            entity.CreatedBy = userId;
                            entity.ModifiedAt = timestamp;
                            entity.ModifiedBy = userId;
                            break;
                        case EntityState.Modified:
                            entity.ModifiedAt = timestamp;
                            entity.ModifiedBy = userId;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get Tenant information from the connected user and set fields before savings
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="currentUser"></param>
        private static void SetTenantField(IEnumerable<EntityEntry> entities, ICurrentUser currentUser)
        {
            var tenantEntities = entities
                    .Where(t => t.Entity is ITenantEntity &&
                    (
                        t.State == EntityState.Added || t.State == EntityState.Modified
                    ));

            if (tenantEntities.Any())
            {
                foreach (EntityEntry entry in tenantEntities)
                {
                    ITenantEntity entity = (ITenantEntity)entry.Entity;
                    entity.TenantId = currentUser.TenantId;
                }
            }
        }

        /// <summary>
        /// Update version (concurrency field) before update or add
        /// </summary>
        /// <param name="entries"></param>
        private static void SetConcurrencyField(IEnumerable<EntityEntry> entries)
        {
            var concurrencyEntities = entries
                .Where(t => t.Entity is IConcurrencyCheckEntity &&
                    (
                        t.State == EntityState.Added || t.State == EntityState.Modified
                    ));

            if (concurrencyEntities.Any())
            {
                foreach (EntityEntry entry in concurrencyEntities)
                {
                    IConcurrencyCheckEntity entity = (IConcurrencyCheckEntity)entry.Entity;
                    entity.Version = Guid.NewGuid();
                }
            }
        }
    }
}
