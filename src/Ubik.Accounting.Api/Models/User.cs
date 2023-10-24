using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    //Will be read only, all data will be updated by message broker
    //Will see if we really need a specific user table for this microservices 
    //The claims seem to be sufficient.
    //[Index(nameof(TenantId), IsUnique = false)]
    //public class User : ITenantEntity
    //{
    //    public Guid Id { get; set; }
    //    [StringLength(100)]
    //    public required string Name { get; set; }
    //    [StringLength(200)]
    //    public required string Email { get; set; }
    //    public Guid TenantId { get; set; }
    //}
}
