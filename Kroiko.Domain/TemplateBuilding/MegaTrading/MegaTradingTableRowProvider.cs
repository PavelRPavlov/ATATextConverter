namespace Kroiko.Domain.TemplateBuilding.MegaTrading;

public class MegaTradingTableRowProvider: ITableRowProvider
{
    private static readonly List<string> DetailPropertyToColumnMap =
    [
        nameof(MegaTradingDetail.Material),
        nameof(MegaTradingDetail.Thickness),
        nameof(MegaTradingDetail.Height),
        nameof(MegaTradingDetail.Width),
        nameof(MegaTradingDetail.Quantity),
        nameof(MegaTradingDetail.Rotated),
        nameof(MegaTradingDetail.EdgeBandingMaterial),
        nameof(MegaTradingDetail.Note)
    ];
    
    public IEnumerable<Cell> GetTableRow(IKroikoDetail det, int rowNumber, int startColumnNumber)
    {
        var currentColumnNumber = startColumnNumber;
        var result = new List<Cell>();

        var detailType = typeof(MegaTradingDetail);
        var detail = (MegaTradingDetail)det;
        foreach (var property in DetailPropertyToColumnMap)
        {
            currentColumnNumber = ExtractDirectColumnInfo(detail, rowNumber, detailType, property, currentColumnNumber, result);
        }
        
        return result;
    }

    private static int ExtractDirectColumnInfo(MegaTradingDetail detail, int rowNumber, Type detailType, string property,
        int currentColumnNumber, List<Cell> result)
    {
        var info = detailType.GetProperty(property);
        var propertyName = info?.Name;
        var propertyStringValue = info?.GetValue(detail)?.ToString();
        Cell newCell;
        switch (propertyName)
        {
            // NOTE this switch controls how the content of a cell will be aligned
            case nameof(MegaTradingDetail.Material):
            case nameof(MegaTradingDetail.Note):
                newCell = new(Cell.GetCellName(rowNumber, currentColumnNumber))
                {
                    Value = propertyStringValue
                };
                break;
                
            default:
                newCell = new(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
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