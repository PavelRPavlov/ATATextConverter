using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ATAFurniture.Server.Models;
using ATAFurniture.Server.Services.DetailsExtractor;
using ATAFurniture.Server.Services.ExcelGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services.Template.Lonira;

public class LoniraTemplateBuilder(ILogger<LoniraTemplateBuilder> logger, [FromKeyedServices(nameof(SupportedCompanies.Lonira))] ITableRowProvider tableRowProvider, string templateFilePath = null ) : TemplateBuilderBase
{
    private const string MaterialNameCellFlag = "{MaterialName}";
    
    private readonly string _defaultTemplateFilePath = Path.Combine(Environment.CurrentDirectory, "Services", "Template", "Lonira", "template.json");

    public override async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<Detail> details)
    {
        var groupedDetails = details.GroupBy(d => d.Material);
        List<ISheet> sheets = new();
        foreach (var group in groupedDetails)
        {
            if (group.Key.Contains('?'))
            {
                continue;
            }
            
            var groupMaterial = group.Key;
            var groupDetails = group.ToList();
            
            var sheet = await ReadTemplateAsync<LoniraSheet>(templateFilePath ?? _defaultTemplateFilePath);
            sheet.SheetMaterial = groupMaterial;
            
            var tableStartCell = PopulateStaticInfo(sheet, contactInfo);
            PopulateDetails(sheet, tableStartCell, groupDetails, tableRowProvider);
            PopulateMaterialName(sheet, groupMaterial);
            
            sheets.Add(sheet);
        }

        return sheets;
    }
    
    private void PopulateMaterialName(ISheet sheet, string materialName)
    {
        foreach (var cell in sheet.Cells.Where(cell => cell.Value.ToString() == MaterialNameCellFlag))
        {
            cell.Value = materialName;
        }
    }
}