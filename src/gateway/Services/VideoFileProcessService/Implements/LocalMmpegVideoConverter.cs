namespace gateway.Services.VideoFileProcessService.Implements;
using System.Diagnostics;

class LocalMmpegVideoConverter : IVideoConvertProcessor
{
    private readonly ILogger<LocalMmpegVideoConverter> _logger;

    public LocalMmpegVideoConverter(ILogger<LocalMmpegVideoConverter> logger) 
    {
        this._logger = logger;
        if (!File.Exists("ffmpeg"))
        {
            _logger.LogCritical($"The mmpeg was not found. {nameof(LocalMmpegVideoConverter)} can not be executed.");
            throw new InvalidOperationException("The MMpeg file was not found. Process will be terminate");
        }

    }
    public async Task ConvertAsync(string localSrcFileName, string localDestFileName)
    {
        _logger.LogDebug($"Starting convert '{localSrcFileName}' to '{localDestFileName}'");
        var ffmpeg = Process.Start("ffmpeg", $"-i \"{localSrcFileName}\" \"{localDestFileName}\""); //ffmpeg -i input.avi output.mp4
        await ffmpeg.WaitForExitAsync();
    }
}