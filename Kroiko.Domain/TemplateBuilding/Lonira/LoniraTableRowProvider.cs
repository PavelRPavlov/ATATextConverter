using Kroiko.Domain.CellsExtracting;

namespace Kroiko.Domain.TemplateBuilding.Lonira;

public class LoniraTableRowProvider : ITableRowProvider
{
    private static readonly string[] DetailPropertyToColumnMap =
    [
        nameof(LoniraDetail.Height),
        nameof(LoniraDetail.Width),
        nameof(LoniraDetail.Quantity),
        nameof(LoniraDetail.LoniraEdges),
        nameof(LoniraDetail.Note)
    ];
    
    private static readonly string[] CenteredCellsContent =
    [
        nameof(LoniraDetail.Height),
        nameof(LoniraDetail.Width),
        nameof(LoniraDetail.Quantity)
    ];
    
    public IEnumerable<Cell> GetTableRow(IKroikoDetail detail, int rowNumber, int startColumnNumber)
    {
        var result = new List<Cell>();

        var detailType = typeof(LoniraDetail);
        foreach (var property in DetailPropertyToColumnMap)
        {
            var propertyValue = detailType.GetProperty(property)?.GetValue(detail)?.ToString();
            if (string.IsNullOrEmpty(propertyValue))
            {
                propertyValue = "";
            }
            var cellContentAlignment = CenteredCellsContent.Contains(property) ? (byte)1 : (byte)0;
            result.Add(new Cell(Cell.GetCellName(rowNumber, startColumnNumber), cellContentAlignment)
            {
                Value = propertyValue
            });
            
            startColumnNumber++;
        }
        
        return result;
    }
}