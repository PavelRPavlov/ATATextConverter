using System;

namespace ATAFurniture.Server.Services.Template.Suliver;

public class SuliverFileNameProvider : IFileNameProvider
{
    public string GetFileNameForSheet(ISheet sheet)
    { 
        return $"{DateTime.Now:yyyy-MM-dd}_{{CompanyName}}.xlsx";
    }
}