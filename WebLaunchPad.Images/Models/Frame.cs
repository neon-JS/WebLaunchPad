namespace WebLaunchPad.Images.Models;

public record Frame(
    int DurationMs,
    IDictionary<Frame.Coordinate, Color> Mapping
)
{
    public record Coordinate(uint X, uint Y);
};