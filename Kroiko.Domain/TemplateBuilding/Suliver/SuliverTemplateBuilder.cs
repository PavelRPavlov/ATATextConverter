﻿using System.Reflection;
using Kroiko.Domain.CellsExtracting;
using Microsoft.Extensions.DependencyInjection;

namespace Kroiko.Domain.TemplateBuilding.Suliver;

public class SuliverTemplateBuilder([FromKeyedServices(nameof(SupportedCompanies.Suliver))] ITableRowProvider tableRowProvider, string? templatePath = null) : TemplateBuilderBase
{
    private readonly string _defaultTemplateFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "..",
        "TemplateBuilding", "Suliver", "template.json");
    
    public override async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<KroikoFile> files)
    {
        var sheet = await ReadTemplateAsync<SheetBase>(templatePath ?? _defaultTemplateFilePath);
        var tableStartCell = PopulateStaticInfo(sheet, contactInfo);
        
        // TODO Suliver has a single file
        PopulateDetails(sheet, tableStartCell, files.First().Details, tableRowProvider);

        return new List<ISheet> { sheet };
    }
}