using System.Collections.Generic;
using System.Linq;
using LargeXlsx;

namespace ATAFurniture.Server.Services.ExcelGenerator.XlsxWrapper;

public static class ColumnGeneratorExtensions
{
    public static IEnumerable<XlsxColumn> ToColumnStyle(this IEnumerable<int> columnWidths)
    {
        return columnWidths.Select(columnWidth => XlsxColumn.Formatted(width: columnWidth)).ToList();
    }
}