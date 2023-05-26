namespace gateway.Services.VideoFileProcessService.Models;

internal class VideoFile
{
    public VideoFile(string processId, string fileSrcUrl, string fileDestUrl)
    {
        ProcessId = processId;
        FileSrcUrl = fileSrcUrl;
        FileDestUrl = fileDestUrl;
    }

    public string ProcessId { get; }
    public string FileSrcUrl { get; }
    public string FileDestUrl { get; }
}

