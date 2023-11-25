namespace WebLaunchPad.Communication.Models;

public class Color(byte red, byte green, byte blue)
{
    public byte Red { get; } = red;
    public byte Green { get; } = green;
    public byte Blue { get; } = blue;

    public int GetAsRgb()
    {
        return Red * 0x10000 + Green * 0x100 + Blue;
    }
}