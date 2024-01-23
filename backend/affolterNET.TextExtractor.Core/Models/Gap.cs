namespace affolterNET.TextExtractor.Core.Models;

public class Gap
{
    public Gap(double size)
    {
        First = 0;
        Second = size;
    }

    public Gap(double first, double second)
    {
        First = first;
        Second = second;
    }
    
    public double First { get; set; }
    public double Second { get; set; }
    public double Size => Math.Abs(First - Second);
}