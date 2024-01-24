namespace Kroiko.Domain.TemplateBuilding;

public class SheetBase : ISheet
{
    public List<int> ColumnWidths { get; set; }
    public List<Cell> Cells { get; set; }
    
}