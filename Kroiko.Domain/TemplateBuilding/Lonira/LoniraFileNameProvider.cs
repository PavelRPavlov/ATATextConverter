namespace Kroiko.Domain.TemplateBuilding.Lonira;

public class LoniraFileNameProvider : IFileNameProvider
{
    public string GetFileNameForSheet(ISheet sheet)
    { 
        var name = $"{DateTime.Now:yyyy-MM-dd}_{{CompanyName}}_{{Material}}.xlsx";
        if (sheet is LoniraSheet loniraSheet)
        {
            name = name.Replace("{Material}", loniraSheet.SheetMaterial);
        }

        return name;
    }
}