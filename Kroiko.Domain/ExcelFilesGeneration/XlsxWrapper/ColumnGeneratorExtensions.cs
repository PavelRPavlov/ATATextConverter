using LargeXlsx;

namespace Kroiko.Domain.ExcelFilesGeneration.XlsxWrapper;

public static class ColumnGeneratorExtensions
{
    public static IEnumerable<XlsxColumn> ToColumnStyle(this IEnumerable<int> columnWidths)
    {
        return columnWidths.Select(columnWidth => XlsxColumn.Formatted(width: columnWidth)).ToList();
    }
}