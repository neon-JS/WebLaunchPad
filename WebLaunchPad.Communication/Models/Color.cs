namespace WebLaunchPad.Communication.Models;

public class Color
{
    public byte Red { get; private set; }
    public byte Green { get; private set; }
    public byte Blue { get; private set; }

    public Color(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public int GetAsRgb()
    {
        return Red * 0x10000 + Green * 0x100 + Blue;
    }
}