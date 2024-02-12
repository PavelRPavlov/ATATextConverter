using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;

namespace ATAFurniture.Server.Models;

public static class SuliverExtensions
{
    public static List<IKroikoDetail> ToSuliverDetails(this ObservableCollection<Detail> details)
    {
        var result = new List<IKroikoDetail>();
        foreach (var detail in details)
        {
            result.Add(new SuliverDetail
            {
                Id = Guid.NewGuid(),
                Width = detail.Width,
                Height = detail.Height,
                Quantity = detail.Quantity,
                Material = detail.Material,
                Cabinet = detail.Cabinet,
                MaterialThickness = detail.MaterialThickness,
                IsGrainDirectionReversed = detail.IsGrainDirectionReversed ? (byte)2 : (byte)1,
                LongEdge = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness),
                LongEdge2 = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness),
                ShortEdge = GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness),
                ShortEdge2 = GetSuliverEdgeThicknessValue(detail.RightEdgeThickness),
            });
        }
        return result;
    }


    private static byte GetSuliverEdgeThicknessValue(double detailTopEdgeThickness)
    {
        
        return detailTopEdgeThickness switch 
        {
            0 => 0,
            > 0.4 and < 0.6 => 1,
            > 0.7 and < 1.4 => 3,
            > 1.6 and < 2.4 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(detailTopEdgeThickness))
        };
    }
}