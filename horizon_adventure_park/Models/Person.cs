namespace HorizonAdventurePark.Models;

public abstract class Person
{
    public string Id { get; }
    public string FullName { get; }

    protected Person(string id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }
}