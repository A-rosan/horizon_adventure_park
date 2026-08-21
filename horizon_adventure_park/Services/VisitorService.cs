using HorizonAdventurePark.Data;
using HorizonAdventurePark.Enums;
using HorizonAdventurePark.Exceptions;
using HorizonAdventurePark.Models;

namespace HorizonAdventurePark.Services;

public class VisitorService
{
    private readonly ParkData _data;

    public VisitorService(ParkData data)
    {
        _data = data;
    }

    public Visitor RegisterVisitor(
        string id,
        string fullName,
        int age,
        double heightCm,
        VisitorCategory category)
    {
        if (_data.Visitors.Any(v => v.Id.Equals(
            id,
            StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessRuleException(
                "A visitor with this ID already exists.");
        }

        // validate the input parameters if it null or empty

        if (String.IsNullOrWhiteSpace(id)) {
            throw new BusinessRuleException(
                "Visitor ID cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BusinessRuleException(
                "Visitor name cannot be empty.");
        }
        if (String.IsNullOrWhiteSpace(age.ToString())) {
            throw new BusinessRuleException(
                "Visitor age cannot be empty.");
        }

        if (String.IsNullOrWhiteSpace(heightCm.ToString()))
        {
            throw new BusinessRuleException(
                "Visitor height cannot be empty.");
        }

        if (age <= 0)
        {
            throw new BusinessRuleException(
                "Age must not be zero or negative.");
        }

        if (heightCm <= 0)
        {
            throw new BusinessRuleException(
                "Height must be greater than zero.");
        }

        Visitor visitor = new(
            id,
            fullName,
            age,
            heightCm,
            category);

        _data.Visitors.Add(visitor);

        return visitor;
    }

    public Visitor? FindVisitor(string id)
    {
        return _data.Visitors.FirstOrDefault(
            v => v.Id.Equals(
                id,
                StringComparison.OrdinalIgnoreCase));
    }
}