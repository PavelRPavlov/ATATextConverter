using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ATAFurniture.Server.Models;
using ATAFurniture.Server.Services.DetailsExtractor;
using ATAFurniture.Server.Services.ExcelGenerator;

namespace ATAFurniture.Server.Services.Template;

public abstract class TemplateBuilderBase : ITemplateBuilder
{
    private const string CompanyNameCellFlag = "{CompanyName}";
    private const string MobileNumberCellFlag = "{MobileNumber}";
    private const string TableStartCellFlag = "{TableStart}";

    public abstract Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<Detail> details);

    protected Cell PopulateStaticInfo(ISheet sheet, ContactInfo contactInfo)
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
    
    protected async Task<T> ReadTemplateAsync<T>(string templatePath) where T : ISheet
    {
        var rawContent = await File.ReadAllTextAsync(templatePath);
        var templateDefinition = JsonSerializer.Deserialize<T>(rawContent);
        return templateDefinition;
    }
    
    protected void PopulateDetails(ISheet sheet, Cell tableStartCell, IEnumerable<Detail> details, ITableRowProvider tableRowProvider)
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