using Kroiko.Domain.CellsExtracting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Kroiko.Domain.TemplateBuilding.MegaTrading;

public class MegaTradingTemplateBuilder([FromKeyedServices(nameof(SupportedCompanies.MegaTrading))] ITableRowProvider tableRowProvider, string? templatePath = null): TemplateBuilderBase
{
    private readonly string _defaultTemplateFilePath = "Kroiko.Domain.TemplateBuilding.MegaTrading.template.json"; 
        //Path.Combine(Assembly.GetExecutingAssembly().Location, "TemplateBuilding", "MegaTrading", "template.json");

    public override async Task<IList<ISheet>> BuildTemplateAsync(IEnumerable<KroikoFile> files)
    {
        var sheet = await ReadTemplateAsync<SheetBase>(templatePath ?? _defaultTemplateFilePath);
        var tableStartCell = PopulateStaticInfo(sheet);
        
        // NOTE MegaTrading has a single file
        PopulateDetails(sheet, tableStartCell, files.First().Details, tableRowProvider);

        return new List<ISheet> { sheet };
    }
}