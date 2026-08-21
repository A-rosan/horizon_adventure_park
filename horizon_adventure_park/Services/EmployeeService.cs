using HorizonAdventurePark.Data;
using HorizonAdventurePark.Exceptions;
using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Services;

public class EmployeeService
{
    private readonly ParkData _data;

    public EmployeeService(ParkData data)
    {
        _data = data;
    }

    public Employee AddEmployee(
        string id,
        string fullName,
        Enums.EmployeeRole role)
    {
        if (_data.Employees.Any(e =>
            e.Id.Equals(
                id,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessRuleException(
                "An employee with this ID already exists.");
        }

        Employee employee = new(
            id,
            fullName,
            role);

        _data.Employees.Add(employee);

        return employee;
    }

    public void AssignEmployee(
        string employeeId,
        string locationId,
        string timeSlot)
    {
        Employee? employee =
            _data.Employees.FirstOrDefault(
                e => e.Id.Equals(
                    employeeId,
                    StringComparison.OrdinalIgnoreCase));

        if (employee == null)
        {
            throw new BusinessRuleException(
                "Employee does not exist.");
        }

        bool alreadyAssigned =
            _data.StaffAssignments.Any(a =>
                a.EmployeeId.Equals(
                    employeeId,
                    StringComparison.OrdinalIgnoreCase) &&
                a.TimeSlot.Equals(
                    timeSlot,
                    StringComparison.OrdinalIgnoreCase));

        if (alreadyAssigned)
        {
            throw new BusinessRuleException(
                "Employee is already assigned to another location during this time slot.");
        }

        StaffAssignment assignment = new(
            employeeId,
            locationId,
            timeSlot);

        _data.StaffAssignments.Add(assignment);
    }
}