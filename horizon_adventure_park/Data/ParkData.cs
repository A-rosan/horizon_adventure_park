using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Data;

public class ParkData
{
    public List<Visitor> Visitors { get; } = new(); // List to store Visitor objects

    public List<Employee> Employees { get; } = new(); // List to store Employee objects

    public List<Ticket> Tickets { get; } = new(); // List to store Ticket objects

    public List<Ride> Rides { get; } = new(); // List to store Ride objects

    public List<Reservation> Reservations { get; } = new(); // List to store Reservation objects

    public List<StaffAssignment> StaffAssignments { get; } = new(); // List to store StaffAssignment objects  
}