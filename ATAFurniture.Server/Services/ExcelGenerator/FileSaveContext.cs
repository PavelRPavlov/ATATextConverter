namespace ATAFurniture.Server.Services.ExcelGenerator;

public class FileSaveContext(string fileName, byte[] content)
{
    public byte[] Content { get; init; } = content;

    public string FileName { get; set; } = fileName;
}