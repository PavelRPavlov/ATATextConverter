using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ATAFurniture.Server.Models;
using ATAFurniture.Server.Services.ExcelGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services.Template.Suliver;

public class SuliverTemplateBuilder(ILogger<SuliverTemplateBuilder> logger, [FromKeyedServices(nameof(SupportedCompanies.Suliver))] ITableRowProvider tableRowProvider, string templatePath = null) : TemplateBuilderBase
{
    private readonly string _defaultTemplateFilePath = Path.Combine(Environment.CurrentDirectory, "Services", "Template", "Suliver", "template.json");
    
    public override async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<Detail> details)
    {
        var sheet = await ReadTemplateAsync<SheetBase>(templatePath ?? _defaultTemplateFilePath);
        var tableStartCell = PopulateStaticInfo(sheet, contactInfo);
        PopulateDetails(sheet, tableStartCell, details, tableRowProvider);

        return new List<ISheet> { sheet };
    }
}