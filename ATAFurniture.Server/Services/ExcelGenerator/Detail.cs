
namespace ATAFurniture.Server.Services.ExcelGenerator;

public struct Detail
{
    public double Width;
    public double Height;
    public int Quantity;
    public int CuttingNumber;
    public string Material;
    public string Cabinet;
    public bool IsGrainDirectionReversed;
    public bool HasLeftEdge;
    public bool HasTopEdge;
    public bool HasRightEdge;
    public bool HasBottomEdge;
    public string LoniraEgdes => CreateLoniraEdges();

    public Detail(double height, double width, int quantity, string material, bool isGrainDirectionReversed,
        bool hasTopEdge, bool hasBottomEdge, bool hasRightEdge, bool hasLeftEdge, string cabinet, int cuttingNumber)
    {
        Height = height;
        Width = width;
        Quantity = quantity;
        Material = material;
        IsGrainDirectionReversed = isGrainDirectionReversed;
        HasLeftEdge = hasLeftEdge;
        HasTopEdge = hasTopEdge;
        HasRightEdge = hasRightEdge;
        HasBottomEdge = hasBottomEdge;
        Cabinet = cabinet;
        CuttingNumber = cuttingNumber;

        if (IsGrainDirectionReversed)
        {
            (Width, Height) = (Height, Width);
        }
    }

    private string CreateLoniraEdges()
    {
        int longEdgeCount = 0;
        int shortEdgeCount = 0;

        if (Width >= Height)
        {
            if (HasLeftEdge)
            {
                shortEdgeCount++;
            }

            if (HasTopEdge)
            {
                longEdgeCount++;
            }

            if (HasRightEdge)
            {
                shortEdgeCount++;
            }

            if (HasBottomEdge)
            {
                longEdgeCount++;
            }
        }
        else
        {
            if (HasLeftEdge)
            {
                longEdgeCount++;
            }

            if (HasTopEdge)
            {
                shortEdgeCount++;
            }

            if (HasRightEdge)
            {
                longEdgeCount++;
            }

            if (HasBottomEdge)
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
