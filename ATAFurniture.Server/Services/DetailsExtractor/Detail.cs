namespace ATAFurniture.Server.Services.DetailsExtractor;

public record Detail(
    double Height,
    double Width,
    int Quantity,
    string Material,
    bool IsGrainDirectionReversed,
    bool HasTopEdge,
    bool HasBottomEdge,
    bool HasRightEdge, 
    bool HasLeftEdge, 
    string Cabinet, 
    int CuttingNumber,
    double MaterialThickness,
    double TopEdgeThickness,
    double BottomEdgeThickness,
    double RightEdgeThickness,
    double LeftEdgeThickness);