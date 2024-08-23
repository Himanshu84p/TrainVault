using System;
using System.Collections.Generic;

namespace TrainVault.DataAccess;

public partial class Training
{
    public int TrainingId { get; set; }

    public DateOnly DateOfTraining { get; set; }

    public string PlaceOfTraining { get; set; } = null!;

    public string PurposeOfTraining { get; set; } = null!;

    public int OrganizationId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
