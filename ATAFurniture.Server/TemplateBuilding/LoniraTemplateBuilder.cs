using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kroiko.Domain;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.Lonira;
using Microsoft.Extensions.DependencyInjection;

namespace ATAFurniture.Server.TemplateBuilding.Lonira;

public class LoniraTemplateBuilder([FromKeyedServices(nameof(SupportedCompanies.Lonira))] ITableRowProvider tableRowProvider, string? templateFilePath = null ) : TemplateBuilderBase
{
    private const string MaterialNameCellFlag = "{MaterialName}";

    private readonly string _defaultTemplateFilePath =
        Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "TemplateBuilding", "Lonira", "template.json");

    public override async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<KroikoFile> files)
    {
        List<ISheet> sheets = new();
        foreach (var file in files)
        {
            // TODO only for Lonira the details are grouped by material beforehand 
            var groupMaterial = file.FileName;
            var groupDetails = file.Details.Cast<LoniraDetail>();
            
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
        foreach (var cell in sheet.Cells.Where(cell => cell.Value?.ToString() == MaterialNameCellFlag))
        {
            cell.Value = materialName;
        }
    }
}