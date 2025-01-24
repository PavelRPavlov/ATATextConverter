namespace Kroiko.Domain.TemplateBuilding.MegaTrading;

public class MegaTradingDetail : IKroikoDetail
{
    public string? Note { get; set; }
    public string Material { get; set; }
    public string? EdgeBandingMaterial { get; set; }
    public bool Rotated { get; set; }
    public string LeftEdge { get; set; }
    public string RightEdge { get; set; }
    public string BottomEdge { get; set; }
    public string TopEdge { get; set; }
    public Guid Id { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Thickness { get; set; }
    public int Quantity { get; set; }
}