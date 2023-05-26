namespace gateway.Services.FileStoreService.Implements;

internal class FileStoreService : IFileStoreService
{
    public Task RemoveFileAsync(string filePath)
    {
        File.Delete(filePath);
        return Task.FromResult(true);
    }

    public async Task<string> StoreFileAsync(Stream stream, bool closeStream)
    {
        try
        {
            var tempFileName = Path.GetTempFileName();
            using (var tempFileStream = File.Create(tempFileName))
            {
                await stream.CopyToAsync(tempFileStream);
            }
            
            return tempFileName;
        }
        finally
        {
            if (closeStream)
                stream.Close();
        }
    }
}