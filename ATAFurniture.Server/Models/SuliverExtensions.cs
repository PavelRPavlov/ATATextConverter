using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;

namespace ATAFurniture.Server.Models;

public static class SuliverExtensions
{
    private const string NutLamEdgeMaterialName = "NutLam";
    private const string NutEdgeMaterialName = "Nut";
    private const string NutBlumEdgeMaterialName = "NutBlum";
    private const string FalcEdgeMaterialName = "Falc";
    private const string DifferentEdgeMaterialName = "Different";
    
    public static List<IKroikoDetail> ToSuliverDetails(this ObservableCollection<Detail> details)
    {
        var result = new List<IKroikoDetail>();
        foreach (var detail in details)
        {
            var d = new SuliverDetail
            {
                Id = Guid.NewGuid(),
                Width = detail.Width,
                Height = detail.Height,
                Quantity = detail.Quantity,
                Material = detail.Material,
                Cabinet = $"{detail.Cabinet} {detail.Reference}",
                MaterialThickness = detail.MaterialThickness,
                IsGrainDirectionReversed = detail.IsGrainDirectionReversed ? (byte)2 : (byte)1,
                Note = CreateSaliverNote(detail)
            };
            SetSaliverEdges(d, detail);
            result.Add(d);
        }
        return result;
    }

    private static void SetSaliverEdges(SuliverDetail suliverDetail, Detail detail)
    {
        if (detail.Width > detail.Height)
        {
            suliverDetail.LongEdge = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness, detail.TopEdgeMaterial);
            suliverDetail.LongEdge2 = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness, detail.BottomEdgeMaterial);
            suliverDetail.ShortEdge = GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness, detail.LeftEdgeMaterial);
            suliverDetail.ShortEdge2 = GetSuliverEdgeThicknessValue(detail.RightEdgeThickness, detail.RightEdgeMaterial);
        }
        else
        {
            suliverDetail.LongEdge = GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness, detail.LeftEdgeMaterial);
            suliverDetail.LongEdge2 = GetSuliverEdgeThicknessValue(detail.RightEdgeThickness, detail.RightEdgeMaterial);
            suliverDetail.ShortEdge = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness, detail.TopEdgeMaterial);
            suliverDetail.ShortEdge2 = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness, detail.BottomEdgeMaterial);
        }
    }

    private static string CreateSaliverNote(Detail detail)
    {
        var note = new StringBuilder();
        if (detail.OversizingHeight.Equals(detail.OversizingWidth) && detail.OversizingHeight > 0)
        {
            note.Append($"ЗДВ с краен размер {detail.Height - detail.OversizingHeight}x{detail.Width - detail.OversizingWidth}; ");
        }

        if (detail.TopEdgeMaterial.Contains(DifferentEdgeMaterialName) ||
            detail.BottomEdgeMaterial.Contains(DifferentEdgeMaterialName) || 
            detail.LeftEdgeMaterial.Contains(DifferentEdgeMaterialName) ||
            detail.RightEdgeMaterial.Contains(DifferentEdgeMaterialName))
        {
            note.Append("Кантиране с друг цвят");
        }

        return note.ToString();
    }


    private static string GetSuliverEdgeThicknessValue(double detailEdgeThickness, string detailEdgeMaterial)
    {
        if (detailEdgeMaterial == NutLamEdgeMaterialName)
        {
            return "НутЛам";
        }
        if (detailEdgeMaterial == NutEdgeMaterialName)
        {
            return "Нут10x4";
        }
        if (detailEdgeMaterial == FalcEdgeMaterialName)
        {
            return "Фалц13x4";
        }
        if (detailEdgeMaterial == NutBlumEdgeMaterialName)
        {
            return "НутБлум";
        }
        return detailEdgeThickness switch 
        {
            0 => "0",
            > 0.4 and < 0.6 => "1",
            > 0.7 and < 1.4 => "3",
            > 1.6 and < 2.4 => "2",
            _ => throw new ArgumentOutOfRangeException(nameof(detailEdgeThickness))
        };
    }
}