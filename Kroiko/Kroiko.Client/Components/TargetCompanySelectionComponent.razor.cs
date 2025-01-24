using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Kroiko.Client.Components;

public partial class TargetCompanySelectionComponent
{
    [Parameter] public required ConverterContext Context { get; set; }
    
    private readonly ObservableCollection<SupportedCompany> _supportedCompanies = 
        [
            SupportedCompanies.Lonira,
            SupportedCompanies.MegaTrading,
            SupportedCompanies.Suliver,
            SupportedCompanies.SuliverKuklensko
        ];
}