using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kroiko.Domain;

public class ContactInfo : INotifyPropertyChanged
{
    private string? _companyName;
    private string? _mobileNumber;
    private string? _email;

    public string? CompanyName
    {
        get => _companyName;
        set => SetField(ref _companyName, value);
    }

    public string? MobileNumber
    {
        get => _mobileNumber;
        set => SetField(ref _mobileNumber, value);
    }

    public string? Email
    {
        get => _email;
        set => SetField(ref _email, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}