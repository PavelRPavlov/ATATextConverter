using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ATAFurniture.Server.Models;

public static class MegaTradingExtensions {
    public static List<IKroikoDetail> ToMegaTradingDetails(this ObservableCollection<Detail> details)
    {
        var result = new List<IKroikoDetail>();
        foreach (var detail in details)
        {
            result.Add(new MegaTrading
            {
                Id = Guid.NewGuid(),
                Width = detail.Width,
                Height = detail.Height,
                Quantity = detail.Quantity,
                Material = detail.Material,
                Thickness = detail.MaterialThickness,
                // TODO display warning in the UI if the edge materials differ in Polyboard
                EdgeBandingMaterial = HasAnyEdgeSet(detail),
                Rotated = detail.IsGrainDirectionReversed,
                Note = string.Empty,
                // TODO edge material should hold the overall thickness of the edge banding (e.g. 22,28 or 42)
                LeftEdge = detail.HasLeftEdge ? $"{detail.LeftEdgeMaterial}/{GetEdgeBandingThickness(detail.LeftEdgeThickness)}" : string.Empty,
                BottomEdge = detail.HasBottomEdge ? $"{detail.BottomEdgeMaterial}/{GetEdgeBandingThickness(detail.BottomEdgeThickness)}" : string.Empty,
                RightEdge = detail.HasRightEdge ? $"{detail.RightEdgeMaterial}/{GetEdgeBandingThickness(detail.RightEdgeThickness)}" : string.Empty,
                TopEdge = detail.HasTopEdge ? $"{detail.TopEdgeMaterial}/{GetEdgeBandingThickness(detail.TopEdgeThickness)}" : string.Empty,
            });
        }

        return result;
    }
    private static string HasAnyEdgeSet(Detail detail) => 
        detail.HasBottomEdge || detail.HasLeftEdge || detail.HasRightEdge || detail.HasTopEdge ? detail.Material : string.Empty;
    private static string GetEdgeBandingThickness(double detailLeftEdgeThickness) =>
        detailLeftEdgeThickness switch
        {
            <= 0.5 => "0.5",
            > 0.5 and < 1 => "0.8/1.0",
            >= 1 => "2.0",
            _ => ""
        };
}