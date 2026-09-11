namespace AI.SocialNetwork.Domain.ValueObjects;

// Геолокация (Value Object)
public class Coordinates
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}