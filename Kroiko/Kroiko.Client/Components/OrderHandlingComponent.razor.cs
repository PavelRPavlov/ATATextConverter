using Azure.Storage.Blobs;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.ExcelFilesGeneration;
using Kroiko.Domain.Models;
using Kroiko.Domain.TemplateBuilding;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using Task = System.Threading.Tasks.Task;

namespace Kroiko.Client.Components;

public partial class OrderHandlingComponent
{
    [Inject] private IFileGenerator FileGenerator { get; set; }
    [Inject] private IServiceProvider ServiceProvider { get; set; }
    [Inject] private ILogger<OrderHandlingComponent> Logger { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Parameter] public required ConverterContext Context { get; set; }
    
    private bool _areFilesGenerating = false;
    private readonly List<DirectoryInfo> _sessionDirs = [];
    private List<FileDisplayContext> _files = [];
    
    protected override void OnInitialized()
    {
        Context.PropertyChanged += ContextOnPropertyChanged;
    }
    private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ConverterContext.Files))
        {
            _files.Clear();
        }
        StateHasChanged();
    }
    private async Task OnCreateFilesOptionSelected()
    {
        var alreadyGeneratedFiles = await GenerateFiles();
        _files = await SaveFilesForDownload(alreadyGeneratedFiles);
        _areFilesGenerating = false;
        StateHasChanged();
    }
    private async Task<List<FileSaveContext>> GenerateFiles()
    {
        var fileNameProvider = ServiceProvider.GetKeyedService<IFileNameProvider>(Context.TargetCompany.Name);
        if (fileNameProvider is null)
        {
            Logger.LogWarning("No file name provider found for company {CompanyName}", Context.TargetCompany.Name);
        }

        var templateBuilder = ServiceProvider.GetKeyedService<ITemplateBuilder>(Context.TargetCompany.Name) as ITemplateBuilder;
        if (templateBuilder is null)
        {
            Logger.LogWarning("No template builder found for company {CompanyName}", Context.TargetCompany.Name);
        }

        _areFilesGenerating = true;
        // var alreadyGeneratedFiles = await FileGenerator.CreateFiles(
        //     Context.Files,
        //     templateBuilder,
        //     fileNameProvider,
        //     Context.TargetCompany.Name == SupportedCompanies.MegaTrading.Name);

        var alreadyGeneratedFiles = new List<FileSaveContext>();
        if (Context.TargetCompany.Name == SupportedCompanies.MegaTrading.Name)
        {
            alreadyGeneratedFiles.AddRange(await FileGenerator.CreateTextFiles(Context.Files));
        }
        alreadyGeneratedFiles.AddRange(await FileGenerator.CreateExcelFiles(Context.Files,templateBuilder,fileNameProvider));
        return alreadyGeneratedFiles;
    }
    private async Task<List<FileDisplayContext>> SaveFilesForDownload(List<FileSaveContext> alreadyGeneratedFiles)
    {
        var result = new List<FileDisplayContext>();
        var storageConnectionString = Configuration["AzureStorageConnectionString"];
        var storageContainerName = Configuration["StorageContainerName"];
        
        var blobContainer = new BlobContainerClient(storageConnectionString, storageContainerName);
        foreach (var file in alreadyGeneratedFiles)
        {
            result.Add(await CreateFileDownloadLink(blobContainer, file));
        }

        return result;
    }
    private async Task<FileDisplayContext> CreateFileDownloadLink(BlobContainerClient container, FileSaveContext context)
    {
        var filesRootPath = "files";
        var salt = Guid.NewGuid().ToString();
        var sessionFolder = Path.Combine(
            Environment.CurrentDirectory,
            filesRootPath,
            salt);
        var sessionDir = Directory.CreateDirectory(sessionFolder);
        _sessionDirs.Add(sessionDir);

        await File.WriteAllBytesAsync(
            Path.Combine(
                sessionDir.FullName,
                context.FileName),
            context.Content);

        await container.DeleteBlobIfExistsAsync(context.FileName);
        var response = await container.UploadBlobAsync(context.FileName, BinaryData.FromBytes(context.Content));
        
        var blobClient = container.GetBlobClient(context.FileName);
        return new(context.FileName, $"{blobClient.Uri}");
    }

    public void Dispose()
    {
        Context.PropertyChanged -= ContextOnPropertyChanged;
        _files.Clear();

        foreach (var sessionDir in _sessionDirs)
        {
            sessionDir.Delete(true);
        }
    }
    
    // private async Task SendEmailUsingSendInBlue()
    // {
    //     var settings = new EmailSettings();
    //     Configuration.GetSection("EmailSettings").Bind(settings);
    //     var client = new TransactionalEmailsApi();
    //     client.Configuration.ApiKey["api-key"] = settings.ApiKey;
    //     var files = await GenerateFiles();
    //     var usedMaterials = Context.Details.Select(d => d.Material).Distinct().ToList();
    //     _toEmail = _isTestEmail ?
    //         new SendSmtpEmailTo(Context.ContactInfo.Email) :
    //         new SendSmtpEmailTo(Context.TargetCompany.Email, Context.TargetCompany.Name);
    //     var email = new SendSmtpEmail(
    //         new SendSmtpEmailSender(settings.Name, settings.Email),
    //         [_toEmail],
    //         templateId: settings.EmailTemplateId,
    //         _params: new Dictionary<string, object>
    //         {
    //             {
    //                 "CompanyName", Context.ContactInfo.CompanyName
    //             },
    //             {
    //                 "CompanyMobile", Context.ContactInfo.MobileNumber
    //             },
    //             {
    //                 "CompanyEmail", Context.ContactInfo.Email
    //             },
    //             {
    //                 "MaterialList", usedMaterials
    //             }
    //         },
    //         attachment: files.Select(f => new SendSmtpEmailAttachment
    //         {
    //             Name = f.FileName,
    //             Content = f.Content
    //         }).ToList()
    //     );
    //
    //     try
    //     {
    //         await client.SendTransacEmailAsync(email);
    //         _isEmailSentSuccessful = true;
    //     }
    //     catch (Exception e)
    //     {
    //         _isEmailSentSuccessful = false;
    //     }
    // }
}