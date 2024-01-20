namespace ATAFurniture.Server.Models;

public static class SupportedCompanies
{
    public static readonly SupportedCompany Lonira = new SupportedCompany(nameof(Lonira), "Лонира, гр.София");
    public static readonly SupportedCompany Suliver = new SupportedCompany(nameof(Suliver), "Съливер, гр.Пловдив");
}

public record SupportedCompany(string Name, string Translation);