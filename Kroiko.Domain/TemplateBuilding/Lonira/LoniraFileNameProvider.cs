namespace Kroiko.Domain.TemplateBuilding.Lonira;

public class LoniraFileNameProvider : IFileNameProvider
{
    public string GetFileNameForSheet(ISheet sheet)
    { 
        // TODO legacy ???
        // var name = $"{DateTime.Now:yyyy-MM-dd}_{{CompanyName}}_{{Material}}.xlsx";
        
        var name = $"{TemplateBuilderBase.MaterialNameCellFlag}.xlsx";
        if (sheet is LoniraSheet loniraSheet)
        {
            name = name.Replace($"{TemplateBuilderBase.MaterialNameCellFlag}", loniraSheet.SheetMaterial);
        }

        return name;
    }
}