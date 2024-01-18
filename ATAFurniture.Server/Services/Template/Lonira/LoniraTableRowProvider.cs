using System.Collections.Generic;
using ATAFurniture.Server.Services.ExcelGenerator;

namespace ATAFurniture.Server.Services.Template.Lonira;

public class LoniraTableRowProvider : ITableRowProvider
{
    private static readonly List<string> DetailPropertyToColumnMap =
    [
        nameof(Detail.Height), // goes to column 'A'
        nameof(Detail.Width), // goes to column 'B'
        nameof(Detail.Quantity) // goes to column 'C'
    ];
    
    public IEnumerable<Cell> GetTableRow(Detail detail, int rowNumber, int startColumnNumber)
    {
        var result = new List<Cell>();

        var detailType = typeof(Detail);
        foreach (var property in DetailPropertyToColumnMap)
        {
            result.Add(new Cell
            {
                Name = Cell.GetCellName(rowNumber, startColumnNumber),
                Value = detailType.GetProperty(property)?.GetValue(detail)?.ToString(),
                ContentAlignment = 1
            });
            
            startColumnNumber++;
        }
        
        result.Add(new Cell
        {
            Name = Cell.GetCellName(rowNumber, startColumnNumber),
            Value = $"{GetLoniraEdges(detail)}; {detail.Cabinet}"
        });
        
        return result;
    }

    private string GetLoniraEdges(Detail detail)
    {
        if (detail.IsGrainDirectionReversed)
        {
            detail = detail with { Height = detail.Width, Width = detail.Height };
        }
        
        int longEdgeCount = 0;
        int shortEdgeCount = 0;

        if (detail.Width >= detail.Height)
        {
            if (detail.HasLeftEdge)
            {
                shortEdgeCount++;
            }

            if (detail.HasTopEdge)
            {
                longEdgeCount++;
            }

            if (detail.HasRightEdge)
            {
                shortEdgeCount++;
            }

            if (detail.HasBottomEdge)
            {
                longEdgeCount++;
            }
        }
        else
        {
            if (detail.HasLeftEdge)
            {
                longEdgeCount++;
            }

            if (detail.HasTopEdge)
            {
                shortEdgeCount++;
            }

            if (detail.HasRightEdge)
            {
                longEdgeCount++;
            }

            if (detail.HasBottomEdge)
            {
                shortEdgeCount++;
            }
        }

        if (shortEdgeCount == 0 && longEdgeCount == 0)
        {
            return string.Empty;
        }

        if (shortEdgeCount == 0 && longEdgeCount != 0)
        {
            return $"{longEdgeCount} d";
        }

        if (longEdgeCount == 0 && shortEdgeCount != 0)
        {
            return $"{shortEdgeCount} k";
        }

        return $"{shortEdgeCount} k {longEdgeCount} d";
    }
}