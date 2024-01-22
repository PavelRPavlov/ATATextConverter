using System;
using System.Collections.Generic;
using ATAFurniture.Server.Services.DetailsExtractor;
using ATAFurniture.Server.Services.ExcelGenerator;

namespace ATAFurniture.Server.Services.Template.Suliver;

public class SuliverTableRowProvider : ITableRowProvider
{
    private static readonly List<string> DetailPropertyToColumnMap =
    [
        nameof(Detail.Material),
        nameof(Detail.MaterialThickness),
        nameof(Detail.IsGrainDirectionReversed),
        nameof(Detail.Height),
        nameof(Detail.Width),
        nameof(Detail.Quantity),
        nameof(Detail.Cabinet)
        // edges should be added
    ];
    public IEnumerable<Cell> GetTableRow(Detail detail, int rowNumber, int startColumnNumber)
    {
        var currentColumnNumber = startColumnNumber;
        var result = new List<Cell>();

        var detailType = typeof(Detail);
        foreach (var property in DetailPropertyToColumnMap)
        {
            currentColumnNumber = ExtractDirectColumnInfo(detail, rowNumber, detailType, property, currentColumnNumber, result);
        }

        result.AddRange(GetSuliverEdges(detail, rowNumber, currentColumnNumber)) ;
        
        return result;
    }

    private static int ExtractDirectColumnInfo(Detail detail, int rowNumber, Type detailType, string property,
        int currentColumnNumber, List<Cell> result)
    {
        var info = detailType.GetProperty(property);
        var propertyName = info?.Name;
        var propertyStringValue = info?.GetValue(detail)?.ToString();
        Cell newCell;
        switch (propertyName)
        {
            case nameof(Detail.Material):
            case nameof(Detail.Cabinet):
                newCell = new Cell(Cell.GetCellName(rowNumber, currentColumnNumber))
                {
                    Value = propertyStringValue
                };
                break;
                
            case nameof(Detail.IsGrainDirectionReversed):
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

    private IEnumerable<Cell> GetSuliverEdges(Detail detail, int rowNumber, int currentColumnNumber)
    {
        var result = new List<Cell>();
        result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
        {
            Value = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness)
        });
        currentColumnNumber++;
        result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
        {
            Value = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness)
        });
        currentColumnNumber++;
        result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
        {
            Value = GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness)
        });
        currentColumnNumber++;
        result.Add(new Cell(Cell.GetCellName(rowNumber, currentColumnNumber), 1)
        {
            Value = GetSuliverEdgeThicknessValue(detail.RightEdgeThickness)
        });
        return result;
    }

    private string GetSuliverEdgeThicknessValue(double detailTopEdgeThickness)
    {
        return detailTopEdgeThickness switch 
        {
            0 => "",
            0.5 => "1",
            1 => "3",
            2 => "2",
            _ => throw new ArgumentOutOfRangeException(nameof(detailTopEdgeThickness))
        };
    }
}