namespace affolterNET.TextExtractor.Core.Models;

public class FoundGroup<T>: List<GroupItem<T>>
{
    public int GroupId { get; set; }
    public double AvgValue => this.Select(t => t.Value).Average();
    public double MinValue => this.Select(t => t.Value).Min();
    public double MaxValue => this.Select(t => t.Value).Max();
}