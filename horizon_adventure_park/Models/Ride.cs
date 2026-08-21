using HorizonAdventurePark.Enums;

namespace HorizonAdventurePark.Models;

public class Ride
{
    public string Id { get; }
    public string Name { get; }
    public RideType Type { get; }

    public int MinimumAge { get; }
    public double MinimumHeightCm { get; }

    public bool RequiresAdultForChildren { get; }

    public int MaximumCapacity { get; }

    public RideStatus Status { get; private set; }

    public int CurrentOccupancy { get; private set; }

    public Ride(
        string id,
        string name,
        RideType type,
        int minimumAge,
        double minimumHeightCm,
        bool requiresAdultForChildren,
        int maximumCapacity)
    {
        Id = id;
        Name = name;
        Type = type;
        MinimumAge = minimumAge;
        MinimumHeightCm = minimumHeightCm;
        RequiresAdultForChildren = requiresAdultForChildren;
        MaximumCapacity = maximumCapacity;

        Status = RideStatus.Open;
        CurrentOccupancy = 0;
    }

    public void UpdateStatus(RideStatus status)
    {
        Status = status;
    }

    public bool HasCapacity()
    {
        return CurrentOccupancy < MaximumCapacity;
    }

    public void AddVisitor()
    {
        if (!HasCapacity())
        {
            throw new InvalidOperationException(
                "Ride has reached its maximum capacity.");
        }

        CurrentOccupancy++;
    }

    public void RemoveVisitor()
    {
        if (CurrentOccupancy > 0)
        {
            CurrentOccupancy--;
        }
    }
}