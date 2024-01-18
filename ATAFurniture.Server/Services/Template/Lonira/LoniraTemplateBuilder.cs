using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ATAFurniture.Server.Services.ExcelGenerator;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services.Template.Lonira;

public class LoniraTemplateBuilder(ILogger<LoniraTemplateBuilder> logger, ITableRowProvider tableRowProvider, string templateFilePath = null ) : ITemplateBuilder
{
    private const string MaterialNameCellFlag = "{MaterialName}";
    private const string CompanyNameCellFlag = "{CompanyName}";
    private const string MobileNumberCellFlag = "{MobileNumber}";
    private const string TableStartCellFlag = "{TableStart}";
    
    private readonly ITableRowProvider _tableRowProvider = new LoniraTableRowProvider();
    private readonly string _defaultTemplateFilePath = Path.Combine(Environment.CurrentDirectory, "Services", "Template", "Lonira", "template.json");

    public async Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<Detail> details)
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
            
            var sheet = await ReadTemplateAsync(templateFilePath ?? _defaultTemplateFilePath);
            sheet.SheetMaterial = groupMaterial;
            
            var tableStartCell = PopulateStaticInfoAsync(sheet, contactInfo);
            PopulateDetailsAsync(sheet, tableStartCell, groupDetails, _tableRowProvider);
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
    
    private async Task<LoniraSheet> ReadTemplateAsync(string templatePath)
    {
        var rawContent = await File.ReadAllTextAsync(templatePath);
        var templateDefinition = JsonSerializer.Deserialize<LoniraSheet>(rawContent);
        return templateDefinition;
    }

    private Cell PopulateStaticInfoAsync(ISheet sheet, ContactInfo contactInfo)
    {
        var tableStartCell = Cell.Empty;
        foreach (var cell in sheet.Cells)
        {
            switch (cell.Value)
            {
                case CompanyNameCellFlag:
                    cell.Value = contactInfo.CompanyName;
                    break;
                case MobileNumberCellFlag:
                    cell.Value = contactInfo.MobileNumber;
                    break;
                case TableStartCellFlag:
                    tableStartCell = cell;
                    cell.Value = null;
                    break;
                default:
                    break;
            }
        }

        if (tableStartCell != Cell.Empty)
        {
            sheet.Cells.Remove(tableStartCell);
        }

        return tableStartCell;
    }
    
    private void PopulateDetailsAsync(ISheet sheet, Cell tableStartCell, List<Detail> details,
        ITableRowProvider tableRowProvider)
    {
        (int currentRow, int currentColumn) = Cell.GetRowAndColumn(tableStartCell);
        foreach (var detail in details)
        {
            var row = tableRowProvider.GetTableRow(detail, currentRow, currentColumn);
            sheet.Cells.AddRange(row);
            currentRow++;
        }
    }
}