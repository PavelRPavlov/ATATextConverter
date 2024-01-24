using System.Reflection;
using Kroiko.Domain.CellsExtracting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kroiko.Domain.TemplateBuilding.Suliver;

public class SuliverTemplateBuilder(ILogger<SuliverTemplateBuilder> logger, [FromKeyedServices(nameof(SupportedCompanies.Suliver))] ITableRowProvider tableRowProvider, string templatePath = null) : TemplateBuilderBase
{
    private readonly string _defaultTemplateFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "..",
        "TemplateBuilding", "Suliver", "template.json");
    
    public override async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<Detail> details)
    {
        var sheet = await ReadTemplateAsync<SheetBase>(templatePath ?? _defaultTemplateFilePath);
        var tableStartCell = PopulateStaticInfo(sheet, contactInfo);
        PopulateDetails(sheet, tableStartCell, details, tableRowProvider);

        return new List<ISheet> { sheet };
    }
}