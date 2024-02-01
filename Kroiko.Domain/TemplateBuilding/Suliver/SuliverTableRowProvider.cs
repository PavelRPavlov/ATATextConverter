using Kroiko.Domain.CellsExtracting;

namespace Kroiko.Domain.TemplateBuilding.Suliver;

public class SuliverTableRowProvider : ITableRowProvider
{
    private static readonly List<string> DetailPropertyToColumnMap =
    [
        nameof(SuliverDetail.Material),
        nameof(SuliverDetail.MaterialThickness),
        nameof(SuliverDetail.IsGrainDirectionReversed),
        nameof(SuliverDetail.Height),
        nameof(SuliverDetail.Width),
        nameof(SuliverDetail.Quantity),
        nameof(SuliverDetail.Cabinet),
        nameof(SuliverDetail.LongEdge),
        nameof(SuliverDetail.LongEdge2),
        nameof(SuliverDetail.ShortEdge),
        nameof(SuliverDetail.ShortEdge2),
        nameof(SuliverDetail.Note)
    ];
    public IEnumerable<Cell> GetTableRow(IKroikoDetail det, int rowNumber, int startColumnNumber)
    {
        var currentColumnNumber = startColumnNumber;
        var result = new List<Cell>();

        var detailType = typeof(SuliverDetail);
        var detail = (SuliverDetail)det;
        foreach (var property in DetailPropertyToColumnMap)
        {
            currentColumnNumber = ExtractDirectColumnInfo(detail, rowNumber, detailType, property, currentColumnNumber, result);
        }
        
        return result;
    }

    private static int ExtractDirectColumnInfo(SuliverDetail detail, int rowNumber, Type detailType, string property,
        int currentColumnNumber, List<Cell> result)
    {
        var info = detailType.GetProperty(property);
        var propertyName = info?.Name;
        var propertyStringValue = info?.GetValue(detail)?.ToString();
        Cell newCell;
        switch (propertyName)
        {
            case nameof(SuliverDetail.Material):
            case nameof(SuliverDetail.Note):
            case nameof(SuliverDetail.Cabinet):
                newCell = new Cell(Cell.GetCellName(rowNumber, currentColumnNumber))
                {
                    Value = propertyStringValue
                };
                break;
                
            default:
                newCell = new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
                {
                    Value = propertyStringValue
                };
                break;
        }
        result.Add(newCell);
        currentColumnNumber++;
        return currentColumnNumber;
    }
}