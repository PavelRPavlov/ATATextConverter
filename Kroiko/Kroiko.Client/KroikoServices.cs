using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.ExcelFilesGeneration;
using Kroiko.Domain.ExcelFilesGeneration.XlsxWrapper;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.Lonira;
using Kroiko.Domain.TemplateBuilding.MegaTrading;
using Kroiko.Domain.TemplateBuilding.Suliver;
using Kroiko.Domain.TextFileGeneration;

namespace Kroiko.Client;

public static class KroikoServices
{
    public static IServiceCollection AddKroikoServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDetailsExtractor, DetailsExtractor>();
        serviceCollection.AddScoped<IExcelFileGenerator, ExcelFileGenerator>();
        serviceCollection.AddScoped<ITextFileGenerator, MegaTradingFileGenerator>();
        serviceCollection.AddScoped<IFileGenerator, FileGenerator>();
        
        // Lonira support
        serviceCollection.AddKeyedScoped<ITemplateBuilder, LoniraTemplateBuilder>(nameof(SupportedCompanies.Lonira));
        serviceCollection.AddKeyedScoped<ITableRowProvider, LoniraTableRowProvider>(nameof(SupportedCompanies.Lonira));
        serviceCollection.AddKeyedScoped<IFileNameProvider, LoniraFileNameProvider>(nameof(SupportedCompanies.Lonira));
        // Suliver support
        serviceCollection.AddKeyedScoped<ITemplateBuilder, SuliverTemplateBuilder>(nameof(SupportedCompanies.Suliver));
        serviceCollection.AddKeyedScoped<ITableRowProvider, SuliverTableRowProvider>(nameof(SupportedCompanies.Suliver));
        serviceCollection.AddKeyedScoped<IFileNameProvider, SuliverFileNameProvider>(nameof(SupportedCompanies.Suliver));
        // MegaTrading support
        serviceCollection.AddKeyedScoped<ITemplateBuilder, MegaTradingTemplateBuilder>(nameof(SupportedCompanies.MegaTrading));
        serviceCollection.AddKeyedScoped<ITableRowProvider, MegaTradingTableRowProvider>(nameof(SupportedCompanies.MegaTrading));
        serviceCollection.AddKeyedScoped<IFileNameProvider, MegaTradingFileNameProvider>(nameof(SupportedCompanies.MegaTrading));
        return serviceCollection;
    }
}