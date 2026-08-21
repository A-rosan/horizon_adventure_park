using HorizonAdventurePark.Enums;

namespace HorizonAdventurePark.Models;

public class Employee : Person
{
    public EmployeeRole Role { get; }

    public Employee(
        string id,
        string fullName,
        EmployeeRole role)
        : base(id, fullName)
    {
        Role = role;
    }
}