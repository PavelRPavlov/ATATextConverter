namespace Kroiko.Domain.TemplateBuilding;
public class LoniraDetail : IKroikoDetail
{
    public string? LoniraEdges { get; set; }
    public string? Note { get; set; }
    public Guid Id { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
}
public class SuliverDetail : IKroikoDetail
{
    public Guid Id { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
    public string? Material { get; set; }
    public string? Cabinet { get; set; }
    public double MaterialThickness { get; set; }
    public byte IsGrainDirectionReversed { get; set; }
    public byte LongEdge { get; set; }
    public byte LongEdge2 { get; set; }
    public byte ShortEdge { get; set; }
    public byte ShortEdge2 { get; set; }
    public string? Note { get; set; }
}

public interface IKroikoDetail
{
    public Guid Id { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}

public class KroikoFile
{
    public required string FileName { get; set; }
    
    public required List<IKroikoDetail> Details { get; set; }
}