using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Kroiko.Domain.TemplateBuilding;

public abstract class TemplateBuilderBase : ITemplateBuilder
{
    public const string CompanyNameCellFlag = "{CompanyName}";
    public const string MobileNumberCellFlag = "{MobileNumber}";
    public const string TableStartCellFlag = "{TableStart}";
    public const string MaterialNameCellFlag = "{MaterialName}";

    public abstract Task<IList<ISheet>> BuildTemplateAsync(IEnumerable<KroikoFile> files);

    protected Cell PopulateStaticInfo(ISheet sheet)
    {
        var tableStartCell = Cell.Empty;
        foreach (var cell in sheet.Cells)
        {
            switch (cell.Value)
            {
                case CompanyNameCellFlag:
                    cell.Value = "GeneratedByKroiko";
                    break;
                case MobileNumberCellFlag:
                    cell.Value = "GeneratedByKroiko";
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
        //var rawContent = await File.ReadAllTextAsync(templatePath);
        //var rawContent = await new HttpClient().GetStringAsync(templatePath);
        
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(templatePath);
        using var reader = new StreamReader(stream);
        var jsonString = await reader.ReadToEndAsync();
        return JsonSerializer.Deserialize<T>(jsonString);
        
        // var templateDefinition = JsonSerializer.Deserialize<T>(rawContent);
        // return templateDefinition;
    }
    
    protected void PopulateDetails(ISheet sheet, Cell tableStartCell, IEnumerable<IKroikoDetail> details, ITableRowProvider tableRowProvider)
    {
        var (currentRow, currentColumn) = Cell.GetRowAndColumn(tableStartCell);
        foreach (var detail in details)
        {
            var row = tableRowProvider.GetTableRow(detail, currentRow, currentColumn);
            sheet.Cells.AddRange(row);
            currentRow++;
        }
    }
}