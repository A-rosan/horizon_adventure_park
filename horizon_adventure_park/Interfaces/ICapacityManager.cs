using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Interfaces;

public interface ICapacityManager
{
    bool HasAvailableCapacity(Ride ride);
}