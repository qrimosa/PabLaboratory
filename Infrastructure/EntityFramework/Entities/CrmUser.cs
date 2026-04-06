using Microsoft.AspNetCore.Identity;
using AppCore.Interfaces;

namespace Infrastructure.EntityFramework.Entities;

public class CrmUser : IdentityUser, ISystemUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Department { get; set; }
    public required SystemUserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    public void Activate()
    {
        // Change status ONLY if it is currently Inactive
        if (Status == SystemUserStatus.Inactive)
        {
            Status = SystemUserStatus.Active;
        }
    }

    public void Deactivate(DateTime now)
    {
        // Change status ONLY if it is currently Active
        if (Status == SystemUserStatus.Active)
        {
            Status = SystemUserStatus.Inactive;
            DeactivatedAt = now;
        }
    }
}