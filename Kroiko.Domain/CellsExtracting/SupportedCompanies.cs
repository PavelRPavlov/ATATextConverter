namespace Kroiko.Domain.CellsExtracting;

//TODO use SmartEnum instead
public static class SupportedCompanies
{
    public static readonly SupportedCompany Lonira = new SupportedCompany(nameof(Lonira), "Лонира, гр.София", "office@lonyra.com");
    public static readonly SupportedCompany MegaTrading = new SupportedCompany(nameof(MegaTrading), "Мега Трейдинг, гр.София", "razkroi_mt@abv.bg");
    public static readonly SupportedCompany Suliver = new SupportedCompany(nameof(Suliver), "Съливер, гр.Пловдив (бул.Васил Априлов)", "saliver_zaiavki@abv.bg");
    public static readonly SupportedCompany SuliverKuklensko = new SupportedCompany(nameof(Suliver), "Съливер, гр.Пловдив (бул.Кукленско Шосе)", "saliver_m1@abv.bg");
}

public record SupportedCompany(string Name, string Translation, string Email);