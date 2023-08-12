namespace WebLaunchPad.Api.Extensions;

public static class MemoryStreamExtensions
{
    public static bool StartsWith(
        this MemoryStream stream,
        ICollection<byte> compare
    )
    {
        if (stream.Length < compare.Count)
        {
            return false;
        }

        var currentPosition = stream.Position;
        stream.Position = 0;

        var streamBytes = new byte[compare.Count];
        var _ = stream.Read(streamBytes.AsSpan());

        stream.Position = currentPosition;

        return streamBytes.SequenceEqual(compare);
    }
}