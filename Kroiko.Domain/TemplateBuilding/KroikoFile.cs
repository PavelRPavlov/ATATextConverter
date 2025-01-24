namespace Kroiko.Domain.TemplateBuilding;

public sealed class KroikoFile
{
    public required string FileName { get; set; }
    public required List<IKroikoDetail> Details { get; set; }
}