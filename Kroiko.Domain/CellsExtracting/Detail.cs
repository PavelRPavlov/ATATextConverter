using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kroiko.Domain.CellsExtracting;

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
    double LeftEdgeThickness) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}