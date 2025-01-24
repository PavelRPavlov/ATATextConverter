namespace Kroiko.Domain.TemplateBuilding;

public interface IKroikoDetail
{
    public Guid Id { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public string Material { get; set; }
}