using System.Text.Json;

namespace Kroiko.Domain.TemplateBuilding;

public abstract class TemplateBuilderBase : ITemplateBuilder
{
    public const string CompanyNameCellFlag = "{CompanyName}";
    public const string MobileNumberCellFlag = "{MobileNumber}";
    public const string TableStartCellFlag = "{TableStart}";
    public const string DifferentEdgeColorCellFlag = "{DifferentEdgeColor}";

    public abstract Task<IList<ISheet>> BuildTemplateAsync(ContactInfo contactInfo, IEnumerable<KroikoFile> files);

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
    
    protected void PopulateDetails(ISheet sheet, Cell tableStartCell, IEnumerable<IKroikoDetail> details, ITableRowProvider tableRowProvider)
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