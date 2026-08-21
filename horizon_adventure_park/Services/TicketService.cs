using HorizonAdventurePark.Data;
using HorizonAdventurePark.Enums;
using HorizonAdventurePark.Exceptions;
using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Services;

public class TicketService
{
    private readonly ParkData _data;

    public TicketService(ParkData data)
    {
        _data = data;
    }

    public Ticket IssueTicket(
        string visitorId,
        TicketType type,
        decimal price,
        DateTime validFrom,
        DateTime validUntil)
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

        if (!IsTicketCompatible(visitor.Category, type))
        {
            throw new BusinessRuleException(
                $"Ticket type '{type}' is not compatible with visitor category '{visitor.Category}'.");
        }

        if (price < 0)
        {
            throw new BusinessRuleException(
                "Ticket price cannot be negative.");
        }

        if (validUntil <= validFrom)
        {
            throw new BusinessRuleException(
                "Ticket expiration must be after the start date.");
        }

        string ticketId = $"T-{_data.Tickets.Count + 1:D4}";

        Ticket ticket = new(
            ticketId,
            visitorId,
            type,
            price,
            validFrom,
            validUntil);

        _data.Tickets.Add(ticket);

        return ticket;
    }

    public Ticket? GetVisitorTicket(string visitorId)
    {
        return _data.Tickets
            .Where(t =>
                t.VisitorId.Equals(
                    visitorId,
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.ValidUntil)
            .FirstOrDefault();
    }

    public bool ValidateTicket(
        string visitorId,
        out string reason)
    {
        Ticket? ticket = GetVisitorTicket(visitorId);

        if (ticket == null)
        {
            reason = "Visitor does not have a ticket.";
            return false;
        }

        if (ticket.Status == TicketStatus.Cancelled)
        {
            reason = "Visitor's ticket has been cancelled.";
            return false;
        }

        if (ticket.Status == TicketStatus.Expired)
        {
            reason = "Visitor's ticket has expired.";
            return false;
        }

        if (DateTime.Now < ticket.ValidFrom)
        {
            reason = "Visitor's ticket is not active yet.";
            return false;
        }

        if (DateTime.Now > ticket.ValidUntil)
        {
            ticket.Expire();

            reason = "Visitor's ticket has expired.";
            return false;
        }

        reason = "Ticket is valid.";
        return true;
    }

    public void CancelTicket(string visitorId)
    {
        Ticket? ticket = GetVisitorTicket(visitorId);

        if (ticket == null)
        {
            throw new BusinessRuleException(
                "Visitor does not have a ticket.");
        }

        ticket.Cancel();
    }

    private bool IsTicketCompatible(
        VisitorCategory category,
        TicketType ticketType)
    {
        return category switch
        {
            VisitorCategory.General =>
                ticketType == TicketType.Regular,

            VisitorCategory.VIP =>
                ticketType == TicketType.VIP,

            VisitorCategory.Child =>
                ticketType == TicketType.Child,

            VisitorCategory.Senior =>
                ticketType == TicketType.Senior,

            VisitorCategory.StaffAccompaniedMinor =>
                ticketType == TicketType.StaffAccompaniedMinor,

            _ => false
        };
    }
}