namespace Kroiko.Domain.TemplateBuilding.Suliver;

public class SuliverFileNameProvider : IFileNameProvider
{
    public string GetFileNameForSheet(ISheet sheet)
    { 
        return $"{DateTime.Now:yyyy-MM-dd}_{TemplateBuilderBase.CompanyNameCellFlag}.xlsx";
    }
}