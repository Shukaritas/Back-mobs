namespace RentalPeAPI.Property.Domain.Aggregates.Entities;

public class Service
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

   
    public long SpaceId { get; private set; }
    public Space Space { get; private set; } = default!;

    protected Service() { }

    public Service(string name)
    {
        Name = name;
    }
}