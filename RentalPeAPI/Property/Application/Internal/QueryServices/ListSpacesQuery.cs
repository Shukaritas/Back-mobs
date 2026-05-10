namespace RentalPeAPI.Property.Application.Internal.QueryServices;

public class ListSpacesQuery
{
    public int? OwnerId { get; set; }
    public string? Type { get; set; }

    public ListSpacesQuery(int? ownerId = null, string? type = null)
    {
        OwnerId = ownerId;
        Type = type;
    }
}