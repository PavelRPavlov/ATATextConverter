using System.Collections.Generic;

namespace ATAFurniture.Server.Services.Template.Lonira;

public class LoniraSheet : ISheet
{
    public List<int> ColumnWidths { get; set; }
    public List<Cell> Cells { get; set; }
    public string SheetMaterial { get; set; }
}