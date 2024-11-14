using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Models;

namespace Ubik.DB.Common.Extensions
{
    public static class ChangeTrackerExtensions
    {
        //TODO: seems to work, need to manage special deletion when we don't remove a record from the table but only update a bool "deleted" field
        //      new interface ISpecialDelete to be dev => good idea do it.
        public static void SetSpecialFields(this ChangeTracker changeTracker, ICurrentUser currentUser)
        {
            changeTracker.DetectChanges();
            IEnumerable<EntityEntry> entries = changeTracker.Entries();

            SetAuditFields(entries, currentUser);
            SetTenantField(entries, currentUser);
            SetConcurrencyField(entries);
        }

        public static void SetSpecialFieldsForAdminUser(this ChangeTracker changeTracker, ICurrentUser currentUser)
        {
            changeTracker.DetectChanges();
            IEnumerable<EntityEntry> entries = changeTracker.Entries();

            SetAuditFields(entries, currentUser);
            SetConcurrencyField(entries);
        }

        /// <summary>
        /// Set audit trail fields
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="currentUser"></param>
        private static void SetAuditFields(IEnumerable<EntityEntry> entities, ICurrentUser currentUser)
        {
            if (currentUser.Id != Guid.Empty)
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
                                entity.AuditInfo = new AuditData(timestamp, userId, timestamp, userId);
                                break;
                            case EntityState.Modified:
                                entity.AuditInfo = new AuditData(entity.AuditInfo.CreatedAt, entity.AuditInfo.CreatedBy, timestamp, userId);
                                break;
                        }
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
            if (currentUser.Id != Guid.Empty)
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
                        if (currentUser.TenantId == null)
                            throw new InvalidDataException("TenanId is missing, cannot continue");

                        entity.TenantId = (Guid)currentUser.TenantId;
                    }
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
                    entity.Version = NewId.NextGuid();
                }
            }
        }
    }
}
