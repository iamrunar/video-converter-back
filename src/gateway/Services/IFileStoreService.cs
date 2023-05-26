namespace gateway.Services;

public interface IFileStoreService
{
    Task RemoveFileAsync(string filePath);
    Task<string> StoreFileAsync(Stream stream, bool closeStream);
}
