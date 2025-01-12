using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
                RightEdge = detail.HasTopEdge ? $"{detail.TopEdgeMaterial}/{GetEdgeBandingThickness(detail.TopEdgeThickness)}" : string.Empty,
                TopEdge = detail.HasLeftEdge ? $"{detail.LeftEdgeMaterial}/{GetEdgeBandingThickness(detail.LeftEdgeThickness)}" : string.Empty,
                BottomEdge = detail.HasRightEdge ? $"{detail.RightEdgeMaterial}/{GetEdgeBandingThickness(detail.RightEdgeThickness)}" : string.Empty,
                LeftEdge = detail.HasBottomEdge ? $"{detail.BottomEdgeMaterial}/{GetEdgeBandingThickness(detail.BottomEdgeThickness)}" : string.Empty

            });
        }

        return result;
    }
    public static ObservableCollection<MegaTradingViewModel> ToMegaTradingViewModel(this IEnumerable<IKroikoDetail> genericDetails)
    {
        var dtos = genericDetails.Cast<MegaTrading>();
        var vms = dtos.Select(x =>
            new MegaTradingViewModel(x.Rotated, x.EdgeBandingMaterial, x.LeftEdge, x.RightEdge, x.BottomEdge, x.TopEdge, x.Id, x.Width, x.Height, x.Thickness, x.Quantity, x.Material, x.Note));
        return new ObservableCollection<MegaTradingViewModel>(vms);

    }

    public static List<IKroikoDetail> ToKroikoDetails(this ObservableCollection<MegaTradingViewModel> megaTradingViewModels) =>
        megaTradingViewModels.Select(x => new MegaTrading
        {
            Id = x.Id,
            Width = x.Width,
            Height = x.Height,
            Quantity = x.Quantity,
            Material = x.Material,
            BottomEdge = x.BottomEdge,
            LeftEdge = x.LeftEdge,
            RightEdge = x.RightEdge,
            TopEdge = x.TopEdge,
            Note = x.Note,
            Thickness = x.Thickness,
            EdgeBandingMaterial = x.EdgeBandingMaterial,
            Rotated = x.Rotated
        }).ToList<IKroikoDetail>();
    
    private static string HasAnyEdgeSet(Detail detail) =>
        detail.HasBottomEdge || detail.HasLeftEdge || detail.HasRightEdge || detail.HasTopEdge ? detail.Material : string.Empty;
    private static string GetEdgeBandingThickness(double detailLeftEdgeThickness) =>
        detailLeftEdgeThickness switch
        {
            <= 0.5 => "0.5",
            > 0.5 and <= 1 => "0.8/1.0",
            >= 1 => "2.0",
            _ => ""
        };
}