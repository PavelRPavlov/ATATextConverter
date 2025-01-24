namespace Kroiko.Domain.TemplateBuilding.Suliver;

public class SuliverDetail : IKroikoDetail
{
    public Guid Id { get; set; }
    public bool IsEdgeColorDifferent { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
    public string Material { get; set; }
    public string? Cabinet { get; set; }
    public double MaterialThickness { get; set; }
    public byte IsGrainDirectionReversed { get; set; }
    public string? LongEdge { get; set; }
    public string? LongEdge2 { get; set; }
    public string? ShortEdge { get; set; }
    public string? ShortEdge2 { get; set; }
    public string? Note { get; set; }

    public void AdjustHeightForFalc(bool result)
    {
        if (result)
        {
            Height += 4;
        }
    }

    public void AdjustWidthForFalc(bool result)
    {
        if (result)
        {
            Width += 4;
        }
    }
}