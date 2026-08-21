using HorizonAdventurePark.Data;
using HorizonAdventurePark.Enums;
using HorizonAdventurePark.Exceptions;
using HorizonAdventurePark.Interfaces;
using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Services;

public class RideService : IEligibilityChecker, ICapacityManager
{
    private readonly ParkData _data;

    public RideService(ParkData data)
    {
        _data = data;
    }

    public Ride AddRide(
        string id,
        string name,
        RideType type,
        int minimumAge,
        double minimumHeightCm,
        bool requiresAdultForChildren,
        int maximumCapacity)
    {
        if (_data.Rides.Any(r =>
            r.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessRuleException(
                "A ride with this ID already exists.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "Ride name cannot be empty.");
        }

        if (minimumAge < 0)
        {
            throw new BusinessRuleException(
                "Minimum age cannot be negative.");
        }

        if (minimumHeightCm < 0)
        {
            throw new BusinessRuleException(
                "Minimum height cannot be negative.");
        }

        if (maximumCapacity <= 0)
        {
            throw new BusinessRuleException(
                "Ride capacity must be greater than zero.");
        }

        Ride ride = new(
            id,
            name,
            type,
            minimumAge,
            minimumHeightCm,
            requiresAdultForChildren,
            maximumCapacity);

        _data.Rides.Add(ride);

        return ride;
    }

    public Ride? FindRide(string id)
    {
        return _data.Rides.FirstOrDefault(
            r => r.Id.Equals(
                id,
                StringComparison.OrdinalIgnoreCase));
    }

    public void UpdateRideStatus(
        string rideId,
        RideStatus status)
    {
        Ride? ride = FindRide(rideId);

        if (ride == null)
        {
            throw new BusinessRuleException(
                "Ride does not exist.");
        }

        ride.UpdateStatus(status);
    }

    public bool CheckEligibility(
        Visitor visitor,
        Ride ride,
        out string reason)
    {
        if (visitor.Age < ride.MinimumAge)
        {
            reason =
                $"Visitor does not meet the minimum age requirement ({ride.MinimumAge}).";

            return false;
        }

        if (visitor.HeightCm < ride.MinimumHeightCm)
        {
            reason =
                $"Visitor does not meet the minimum height requirement ({ride.MinimumHeightCm}cm).";

            return false;
        }

        if (ride.RequiresAdultForChildren &&
            visitor.Category == VisitorCategory.Child)
        {
            reason =
                "This ride requires an accompanying adult for children.";

            return false;
        }

        reason = "Visitor meets the ride eligibility requirements.";
        return true;
    }

    public bool HasAvailableCapacity(Ride ride)
    {
        return ride.CurrentOccupancy < ride.MaximumCapacity;
    }

    public void AdmitVisitor(
        string visitorId,
        string rideId,
        TicketService ticketService)
    {
        Visitor? visitor = _data.Visitors.FirstOrDefault(
            v => v.Id.Equals(
                visitorId,
                StringComparison.OrdinalIgnoreCase));

        if (visitor == null)
        {
            throw new BusinessRuleException(
                "Visitor does not exist.");
        }

        Ride? ride = FindRide(rideId);

        if (ride == null)
        {
            throw new BusinessRuleException(
                "Ride does not exist.");
        }

        if (ride.Status != RideStatus.Open)
        {
            throw new BusinessRuleException(
                $"Visitor cannot be admitted because the ride is {ride.Status}.");
        }

        if (!ticketService.ValidateTicket(
                visitorId,
                out string ticketReason))
        {
            throw new BusinessRuleException(
                ticketReason);
        }

        Ticket? ticket =
            ticketService.GetVisitorTicket(visitorId);

        if (ticket == null)
        {
            throw new BusinessRuleException(
                "Visitor does not have a valid ticket.");
        }

        if (!CanAccessRide(ticket.Type, ride))
        {
            throw new BusinessRuleException(
                "Visitor's ticket does not provide access to this ride.");
        }

        if (!CheckEligibility(
                visitor,
                ride,
                out string eligibilityReason))
        {
            throw new BusinessRuleException(
                eligibilityReason);
        }

        if (!HasAvailableCapacity(ride))
        {
            throw new BusinessRuleException(
                "Ride has reached its maximum safe capacity.");
        }

        ride.AddVisitor();
    }

    private bool CanAccessRide(
        TicketType ticketType,
        Ride ride)
    {
        if (ticketType == TicketType.VIP)
        {
            return true;
        }

        // Regular/other tickets have access to rides
        // according to the ticket category.
        return ticketType switch
        {
            TicketType.Regular => true,
            TicketType.Child => ride.Type == RideType.Family,
            TicketType.Senior => ride.Type == RideType.Family,
            TicketType.StaffAccompaniedMinor =>
                ride.Type == RideType.Family,

            _ => false
        };
    }
}