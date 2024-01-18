namespace ATAFurniture.Server.Services.ExcelGenerator;

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
    int CuttingNumber);