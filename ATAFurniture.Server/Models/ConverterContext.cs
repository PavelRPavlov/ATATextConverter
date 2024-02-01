﻿#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kroiko.Domain;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;

namespace ATAFurniture.Server.Models;

public sealed class ConverterContext : INotifyPropertyChanged
{
    private ObservableCollection<Detail> _details = new();
    private ObservableCollection<KroikoFile> _files = new();
    private SupportedCompany? _targetCompany = null;
    public ContactInfo ContactInfo { get; set; } = new();
    
    public SupportedCompany? TargetCompany 
    {
        get => _targetCompany;
        set => SetField(ref _targetCompany, value);
    }

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