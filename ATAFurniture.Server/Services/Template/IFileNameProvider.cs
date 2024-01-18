namespace ATAFurniture.Server.Services.Template;

public interface IFileNameProvider
{
    string GetFileNameForSheet(ISheet sheet);
}