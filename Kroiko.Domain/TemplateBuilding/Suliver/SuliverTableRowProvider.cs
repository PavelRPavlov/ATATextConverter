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

        //result.AddRange(GetSuliverEdges(detail, rowNumber, currentColumnNumber)) ;
        
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
            case nameof(SuliverDetail.Cabinet):
                newCell = new Cell(Cell.GetCellName(rowNumber, currentColumnNumber))
                {
                    Value = propertyStringValue
                };
                break;
                
            case nameof(SuliverDetail.IsGrainDirectionReversed):
                var val = bool.Parse(propertyStringValue);
                newCell = new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
                {
                    Value = val ? "2": "1"
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
    
    // private IEnumerable<Cell> GetSuliverEdges(SuliverDetail detail, int rowNumber, int currentColumnNumber)
    // {
    //     var result = new List<Cell>();
    //     result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
    //     {
    //         Value = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness)
    //     });
    //     currentColumnNumber++;
    //     result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
    //     {
    //         Value = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness)
    //     });
    //     currentColumnNumber++;
    //     result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
    //     {
    //         Value = GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness)
    //     });
    //     currentColumnNumber++;
    //     result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
    //     {
    //         Value = GetSuliverEdgeThicknessValue(detail.RightEdgeThickness)
    //     });
    //     return result;
    // }
    //
    // private string GetSuliverEdgeThicknessValue(double detailTopEdgeThickness)
    // {
    //     return detailTopEdgeThickness switch 
    //     {
    //         0 => "",
    //         0.5 => "1",
    //         1 => "3",
    //         2 => "2",
    //         _ => throw new ArgumentOutOfRangeException(nameof(detailTopEdgeThickness))
    //     };
    // }
}