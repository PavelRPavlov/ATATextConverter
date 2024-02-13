﻿using System;
using System.Collections.Generic;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;

namespace ATAFurniture.Server.Models;

public static class LoniraExtensions
{
    public static List<IKroikoDetail> ToLoniraDetails(this List<Detail> details)
    {
        var result = new List<IKroikoDetail>();
        foreach (var detail in details)
        {
            result.Add(new LoniraDetail
            {
                Id = Guid.NewGuid(),
                Width = detail.Width,
                Height = detail.Height,
                Quantity = detail.Quantity,
                LoniraEdges = $"{GetLoniraEdges(detail)}; {detail.Cabinet} {detail.CuttingNumber}",
                Note = CreateLoniraNote(detail)
            });
        }

        return result;
    }

    private static string CreateLoniraNote(Detail detail)
    {
        if (detail.OversizingHeight.Equals(detail.OversizingWidth) && detail.OversizingHeight > 0)
        {
            return $"ЗДВ с краен размер {detail.Height - detail.OversizingHeight}x{detail.Width - detail.OversizingWidth}; ";
        }

        return string.Empty;
    }

    private static string GetLoniraEdges(Detail detail)
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