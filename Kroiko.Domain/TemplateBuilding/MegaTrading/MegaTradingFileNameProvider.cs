namespace Kroiko.Domain.TemplateBuilding.MegaTrading;

public class MegaTradingFileNameProvider: IFileNameProvider
{
    public string GetFileNameForSheet(ISheet sheet) =>
        $"{DateTime.Now:yyyy-MM-dd}_{TemplateBuilderBase.CompanyNameCellFlag}.xlsx";
}