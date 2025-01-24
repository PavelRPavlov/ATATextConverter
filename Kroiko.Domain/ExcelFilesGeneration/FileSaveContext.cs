namespace Kroiko.Domain.ExcelFilesGeneration;

public record FileSaveContext(string FileName, byte[] Content);