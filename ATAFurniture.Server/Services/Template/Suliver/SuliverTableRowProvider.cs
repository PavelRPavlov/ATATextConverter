using System;
using System.Collections.Generic;
using ATAFurniture.Server.Services.ExcelGenerator;

namespace ATAFurniture.Server.Services.Template.Suliver;

public class SuliverTableRowProvider : ITableRowProvider
{
    private static readonly List<string> DetailPropertyToColumnMap =
    [
        nameof(Detail.Material),
        // material thickness is missing
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
        }

        result.AddRange(GetSuliverEdges(detail, rowNumber, currentColumnNumber)) ;
        
        return result;
    }

    private IEnumerable<Cell> GetSuliverEdges(Detail detail, int rowNumber, int currentColumnNumber)
    {
        return null;
    }
}