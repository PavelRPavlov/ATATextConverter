using System;

namespace ATAFurniture.Server.Services.Template.Lonira;

public class LoniraFileNameProvider : IFileNameProvider
{
    public string GetFileNameForSheet(ISheet sheet)
    { 
        var name = $"{DateTime.Now.ToString("yyyy-MM-dd")}_{{CompanyName}}_{{Material}}.xlsx";
        if (sheet is LoniraSheet loniraSheet)
        {
            name = name.Replace("{Material}", loniraSheet.SheetMaterial);
        }

        return name;
    }
}