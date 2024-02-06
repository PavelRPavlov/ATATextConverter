namespace Kroiko.Domain.TemplateBuilding;

public interface ISheet
{
    public List<int> ColumnWidths { get; set; }
    public List<Cell> Cells { get; set; }
}