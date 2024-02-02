namespace affolterNET.TextExtractor.Core.Models;

public class FoundGroups<T>: List<FoundGroup<T>>
{
    private int _groupIdentifier = 0;
    public new void Add(FoundGroup<T> grp)
    {
        grp.GroupId = _groupIdentifier++;
        base.Add(grp);
    }
    
    public new void AddRange(IEnumerable<FoundGroup<T>> grps)
    {
        foreach (var grp in grps)
        {
            Add(grp);
        }
    }
    
    public FoundGroup<T> GetGroup(double value)
    {
        return this.OrderBy(fs => Math.Abs(fs.AvgValue - value)).First();
    }

    public double Min => this.Min(gi => gi.MinValue);
    public double Max => this.Max(gi => gi.MaxValue);
    public double MinMaxDiff => Max - Min;
}