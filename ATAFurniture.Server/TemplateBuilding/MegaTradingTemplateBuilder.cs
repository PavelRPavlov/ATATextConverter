using Kroiko.Domain;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ATAFurniture.Server.TemplateBuilding;

public class MegaTradingTemplateBuilder([FromKeyedServices(nameof(SupportedCompanies.MegaTrading))] ITableRowProvider tableRowProvider, string? templatePath = null): TemplateBuilderBase
{
    private readonly string _defaultTemplateFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "..",
        "TemplateBuilding", "MegaTrading", "template.json");

    public override async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<KroikoFile> files)
    {
        var sheet = await ReadTemplateAsync<SheetBase>(templatePath ?? _defaultTemplateFilePath);
        var tableStartCell = PopulateStaticInfo(sheet, contactInfo);
        
        // NOTE MegaTrading has a single file
        PopulateDetails(sheet, tableStartCell, files.First().Details, tableRowProvider);

        return new List<ISheet> { sheet };
    }
}