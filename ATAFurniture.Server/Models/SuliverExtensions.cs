﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;

namespace ATAFurniture.Server.Models;

public static class SuliverExtensions
{
    private const string NutLamEdgeFlagName = "nutlam";
    private const string NutEdgeFlagName = "nut";
    private const string NutBlumEdgeFlagName = "nutblum";
    private const string FalcEdgeFlagName = "falc";
    private const string DifferentEdgeMaterialName = "different";
    
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
                IsGrainDirectionReversed = detail.IsGrainDirectionReversed ? (byte)2 : (byte)1
            };
            SetSaliverEdges(d, detail);
            CreateSaliverNote(d, detail);
            result.Add(d);
        }
        return result;
    }

    private static void SetSaliverEdges(SuliverDetail suliverDetail, Detail detail)
    {
        if (detail.Width > detail.Height)
        {
            var result = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness, detail.TopEdgeMaterial.ToLowerInvariant());
            suliverDetail.LongEdge2 = result.Value;
            suliverDetail.AdjustHeightForFalc(result.ShouldUpdateSize);
            
            result = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness, detail.BottomEdgeMaterial.ToLowerInvariant());
            suliverDetail.LongEdge = result.Value;
            suliverDetail.AdjustHeightForFalc(result.ShouldUpdateSize);
            
            result= GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness, detail.LeftEdgeMaterial.ToLowerInvariant());
            suliverDetail.ShortEdge2 = result.Value;
            suliverDetail.AdjustWidthForFalc(result.ShouldUpdateSize);
            
            result= GetSuliverEdgeThicknessValue(detail.RightEdgeThickness, detail.RightEdgeMaterial.ToLowerInvariant());
            suliverDetail.ShortEdge = result.Value;
            suliverDetail.AdjustWidthForFalc(result.ShouldUpdateSize);
        }
        else
        {
            var result = GetSuliverEdgeThicknessValue(detail.LeftEdgeThickness, detail.LeftEdgeMaterial.ToLowerInvariant());
            suliverDetail.LongEdge2 = result.Value;
            suliverDetail.AdjustWidthForFalc(result.ShouldUpdateSize);
            
            result = GetSuliverEdgeThicknessValue(detail.RightEdgeThickness, detail.RightEdgeMaterial.ToLowerInvariant());
            suliverDetail.LongEdge = result.Value;
            suliverDetail.AdjustWidthForFalc(result.ShouldUpdateSize);
            
            result = GetSuliverEdgeThicknessValue(detail.TopEdgeThickness, detail.TopEdgeMaterial.ToLowerInvariant());
            suliverDetail.ShortEdge2 = result.Value;
            suliverDetail.AdjustHeightForFalc(result.ShouldUpdateSize);
            
            result = GetSuliverEdgeThicknessValue(detail.BottomEdgeThickness, detail.BottomEdgeMaterial.ToLowerInvariant());
            suliverDetail.ShortEdge = result.Value;
            suliverDetail.AdjustHeightForFalc(result.ShouldUpdateSize);
        }
    }

    private static void CreateSaliverNote(SuliverDetail suliverDetail, Detail detail)
    {
        var note = new StringBuilder();
        if (detail.OversizingHeight.Equals(detail.OversizingWidth) && detail.OversizingHeight > 0)
        {
            note.Append($"СДВ с краен размер {suliverDetail.Height - detail.OversizingHeight}x{suliverDetail.Width - detail.OversizingWidth}; ");
        }

        if (detail.TopEdgeMaterial.ToLowerInvariant().Contains(DifferentEdgeMaterialName) ||
            detail.BottomEdgeMaterial.ToLowerInvariant().Contains(DifferentEdgeMaterialName) || 
            detail.LeftEdgeMaterial.ToLowerInvariant().Contains(DifferentEdgeMaterialName) ||
            detail.RightEdgeMaterial.ToLowerInvariant().Contains(DifferentEdgeMaterialName))
        {
            note.Append("Кантиране с друг цвят");
            suliverDetail.IsEdgeColorDifferent = true;
        }

        suliverDetail.Note = note.ToString();
    }


    private static (string Value, bool ShouldUpdateSize) GetSuliverEdgeThicknessValue(double detailEdgeThickness, string detailEdgeMaterial)
    {
        if (detailEdgeMaterial.Contains(FalcEdgeFlagName))
        {
            return ("Фалц 13x4", true);
        }
        if (detailEdgeMaterial.Contains(NutLamEdgeFlagName))
        {
            return ("Нут Лам", false);
        }
        if (detailEdgeMaterial.Contains(NutBlumEdgeFlagName))
        {
            return ("Нут Блум", false);
        }
        if (detailEdgeMaterial.Contains(NutEdgeFlagName))
        {
            //NOTE this is the most generic name, so it should be last to give the other flags a chance to match
            return ("Нут 10x4", false);
        }
        return detailEdgeThickness switch 
        {
            0 => ("0", false),
            > 0.4 and < 0.6 => ("1", false),
            > 0.7 and < 1.4 => ("3", false),
            > 1.6 and < 2.4 => ("2", false),
            _ => throw new ArgumentOutOfRangeException(nameof(detailEdgeThickness))
        };
    }
}