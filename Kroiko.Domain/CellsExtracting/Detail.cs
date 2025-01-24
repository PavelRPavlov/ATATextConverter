﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kroiko.Domain.CellsExtracting;

public sealed record Detail(
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
    double LeftEdgeThickness,
    string Reference,
    string TopEdgeMaterial,
    string BottomEdgeMaterial,
    string RightEdgeMaterial,
    string LeftEdgeMaterial,
    double OversizingHeight,
    double OversizingWidth) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
}