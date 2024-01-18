using System.Collections.Generic;

namespace ATAFurniture.Server.Services.Template;

public interface ISheet
{
    public List<int> ColumnWidths { get; set; }
    public List<Cell> Cells { get; set; }
}