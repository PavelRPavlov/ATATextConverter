namespace Kroiko.Domain.TemplateBuilding;

public interface ITableRowProvider
{
    IEnumerable<Cell> GetTableRow(IKroikoDetail detail, int rowNumber, int startColumnNumber);
}