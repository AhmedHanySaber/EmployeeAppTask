using System;
using System.Collections.Generic;

namespace EmployeeApp.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public string Department { get; set; } = null!;

    public bool Isactive { get; set; }
}
