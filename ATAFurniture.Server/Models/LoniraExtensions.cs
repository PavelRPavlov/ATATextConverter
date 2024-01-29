using System.Collections.Generic;
using Kroiko.Domain.CellsExtracting;

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
                    Width = detail.Width,
                    Height = detail.Height,
                    Quantity = detail.Quantity,
                    LoniraEdges = GetLoniraEdges(detail)
                });
            }

            return result;
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