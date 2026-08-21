namespace HorizonAdventurePark.Models;

public class StaffAssignment
{
    public string EmployeeId { get; }
    public string LocationId { get; }
    public string TimeSlot { get; }

    public StaffAssignment(
        string employeeId,
        string locationId,
        string timeSlot)
    {
        EmployeeId = employeeId;
        LocationId = locationId;
        TimeSlot = timeSlot;
    }
}