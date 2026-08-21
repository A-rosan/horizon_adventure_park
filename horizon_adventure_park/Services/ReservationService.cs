using HorizonAdventurePark.Data;
using HorizonAdventurePark.Enums;
using HorizonAdventurePark.Exceptions;
using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Services;

public class ReservationService
{
    private readonly ParkData _data;
    private readonly TicketService _ticketService;
    private readonly RideService _rideService;

    public ReservationService(
        ParkData data,
        TicketService ticketService,
        RideService rideService)
    {
        _data = data;
        _ticketService = ticketService;
        _rideService = rideService;
    }

    public Reservation CreateReservation(
        string visitorId,
        string rideId,
        string timeSlot)
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

        Ride? ride = _rideService.FindRide(rideId);

        if (ride == null)
        {
            throw new BusinessRuleException(
                "Ride does not exist.");
        }

        if (ride.Status != RideStatus.Open)
        {
            throw new BusinessRuleException(
                $"Reservations cannot be made because the ride is {ride.Status}.");
        }

        if (string.IsNullOrWhiteSpace(timeSlot))
        {
            throw new BusinessRuleException(
                "Time slot cannot be empty.");
        }

        if (!_ticketService.ValidateTicket(
                visitorId,
                out string ticketReason))
        {
            throw new BusinessRuleException(
                ticketReason);
        }

        Ticket? ticket =
            _ticketService.GetVisitorTicket(visitorId);

        if (ticket == null)
        {
            throw new BusinessRuleException(
                "Visitor does not have a valid ticket.");
        }

        if (_data.Reservations.Any(r =>
            r.VisitorId.Equals(
                visitorId,
                StringComparison.OrdinalIgnoreCase) &&
            r.RideId.Equals(
                rideId,
                StringComparison.OrdinalIgnoreCase) &&
            r.TimeSlot.Equals(
                timeSlot,
                StringComparison.OrdinalIgnoreCase) &&
            r.Status == ReservationStatus.Active))
        {
            throw new BusinessRuleException(
                "Visitor already has a reservation for this ride and time slot.");
        }

        int reservationsForSlot =
            _data.Reservations.Count(r =>
                r.RideId.Equals(
                    rideId,
                    StringComparison.OrdinalIgnoreCase) &&
                r.TimeSlot.Equals(
                    timeSlot,
                    StringComparison.OrdinalIgnoreCase) &&
                r.Status == ReservationStatus.Active);

        if (reservationsForSlot >= ride.MaximumCapacity)
        {
            throw new BusinessRuleException(
                "Ride has reached maximum capacity for the selected time slot.");
        }

        if (!_rideService.CheckEligibility(
                visitor,
                ride,
                out string eligibilityReason))
        {
            throw new BusinessRuleException(
                eligibilityReason);
        }

        string reservationId =
            $"R-{_data.Reservations.Count + 1:D4}";

        Reservation reservation = new(
            reservationId,
            visitorId,
            rideId,
            timeSlot);

        _data.Reservations.Add(reservation);

        return reservation;
    }

    public void CancelReservation(string reservationId)
    {
        Reservation? reservation =
            _data.Reservations.FirstOrDefault(
                r => r.Id.Equals(
                    reservationId,
                    StringComparison.OrdinalIgnoreCase));

        if (reservation == null)
        {
            throw new BusinessRuleException(
                "Reservation does not exist.");
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            throw new BusinessRuleException(
                "Reservation is already cancelled.");
        }

        reservation.Cancel();
    }
}