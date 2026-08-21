using HorizonAdventurePark.Enums;

namespace HorizonAdventurePark.Models;

public class Visitor : Person
{
    public int Age { get; }
    public double HeightCm { get; }
    public VisitorCategory Category { get; }

    public Visitor(
        string id,
        string fullName,
        int age,
        double heightCm,
        VisitorCategory category)
        : base(id, fullName)
    {
        Age = age;
        HeightCm = heightCm;
        Category = category;
    }
}