using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kroiko.Domain.Models;

public sealed class ConverterContext : INotifyPropertyChanged
{
    private ObservableCollection<Detail> _details = [];
    private ObservableCollection<KroikoFile> _files = [];
    private SupportedCompany _targetCompany = SupportedCompanies.MegaTrading;

    public ObservableCollection<Detail> Details
    {
        get => _details;
        set => SetField(ref _details, value);
    }

    public ObservableCollection<KroikoFile> Files
    {
        get => _files;
        set => SetField(ref _files, value);
    }

    public SupportedCompany TargetCompany
    {
        get => _targetCompany;
        set => SetField(ref _targetCompany, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}