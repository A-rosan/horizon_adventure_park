using HorizonAdventurePark.Enums;

namespace HorizonAdventurePark.Models;

public class Reservation
{
    public string Id { get; }
    public string VisitorId { get; }
    public string RideId { get; }
    public string TimeSlot { get; }
    public ReservationStatus Status { get; private set; }

    public Reservation(
        string id,
        string visitorId,
        string rideId,
        string timeSlot)
    {
        Id = id;
        VisitorId = visitorId;
        RideId = rideId;
        TimeSlot = timeSlot;
        Status = ReservationStatus.Active;
    }

    public void Cancel()
    {
        Status = ReservationStatus.Cancelled;
    }
}