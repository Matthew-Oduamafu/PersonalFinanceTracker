using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

#pragma warning disable CS8618

namespace PersonalFinanceTracker.Data.Data.Entities;

public class AppUser : IdentityUser
{
    [MaxLength(50)] public string FirstName { get; set; }

    [MaxLength(50)] public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool IsDeleted { get; set; } = false;

    public bool IsActive { get; set; } = true;
}