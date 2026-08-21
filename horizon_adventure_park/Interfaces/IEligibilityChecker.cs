using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Interfaces;

public interface IEligibilityChecker
{
    bool CheckEligibility(
        Visitor visitor,
        Ride ride,
        out string reason);
}