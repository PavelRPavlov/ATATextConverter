namespace Kroiko.Domain.TemplateBuilding.Lonira;

public class LoniraDetail : IKroikoDetail
{
    public string? Material { get; set; }
    public string? LoniraEdges { get; set; }
    public string? Note { get; set; }
    public Guid Id { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
}