using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;

// Add profile data for application users by adding properties to the DidUFall4It_DDACGroupAssignment_Group21User class
public class DidUFall4It_DDACGroupAssignment_Group21User : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }
    [PersonalData]
    public int Age { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; }
    [PersonalData]
    public string? UserRole { get; set; }
}

