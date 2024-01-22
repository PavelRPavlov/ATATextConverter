using System.Collections.Generic;
using ATAFurniture.Server.Services.DetailsExtractor;
using ATAFurniture.Server.Services.ExcelGenerator;

namespace ATAFurniture.Server.Services.Template;

public interface ITableRowProvider
{
    IEnumerable<Cell> GetTableRow(Detail detail, int rowNumber, int startColumnNumber);
}