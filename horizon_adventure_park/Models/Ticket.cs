using HorizonAdventurePark.Enums;

namespace HorizonAdventurePark.Models;

public class Ticket
{
    public string Id { get; }
    public string VisitorId { get; }
    public TicketType Type { get; }
    public decimal Price { get; }
    public DateTime ValidFrom { get; }
    public DateTime ValidUntil { get; }
    public TicketStatus Status { get; private set; }

    public Ticket(
        string id,
        string visitorId,
        TicketType type,
        decimal price,
        DateTime validFrom,
        DateTime validUntil)
    {
        Id = id;
        VisitorId = visitorId;
        Type = type;
        Price = price;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        Status = TicketStatus.Active;
    }

    public bool IsValid()
    {
        return Status == TicketStatus.Active &&
               DateTime.Now >= ValidFrom &&
               DateTime.Now <= ValidUntil;
    }

    public void Cancel()
    {
        Status = TicketStatus.Cancelled;
    }

    public void Expire()
    {
        Status = TicketStatus.Expired;
    }
}
