namespace affolterNET.TextExtractor.Core.Models;

public class GroupItem<T>
{
    public GroupItem(double val, T obj)
    {
        Value = val;
        Obj = obj;
    }
    
    public double Value { get; set; }
    public T Obj { get; set; }
}