using Kroiko.Domain.CellsExtracting;

namespace Kroiko.Domain.TemplateBuilding;

public interface ITableRowProvider
{
    IEnumerable<Cell> GetTableRow(Detail detail, int rowNumber, int startColumnNumber);
}