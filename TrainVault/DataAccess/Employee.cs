using System;
using System.Collections.Generic;

namespace TrainVault.DataAccess;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int OrganizationId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? JobTitle { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<Training> Training { get; set; } = new List<Training>();
}
