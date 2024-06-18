namespace affolterNET.TextExtractor.Core.Models;

public class BlockNodes
{
    public BlockNodes(List<IGrouping<double, Quadtree>> rows, List<IGrouping<double, Quadtree>> columns)
    {
        Rows = rows;
        Columns = columns;
    }
        
    public List<IGrouping<double, Quadtree>> Rows { get; }
    public List<IGrouping<double, Quadtree>> Columns { get; }
}