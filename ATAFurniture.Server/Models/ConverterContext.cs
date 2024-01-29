#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kroiko.Domain.CellsExtracting;

namespace ATAFurniture.Server.Models;

public sealed class ConverterContext : INotifyPropertyChanged
{
    private ObservableCollection<Detail> _details = new();
    
    public readonly TemporaryContactInfo TempContactInfo = new();
    public SupportedCompany? TargetCompany;

    public ObservableCollection<Detail> Details
    {
        get => _details;
        set => SetField(ref _details, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}