namespace gateway.Services.VideoFileProcessService.Implements;

interface IVideoConvertProcessor
{
    Task ConvertAsync(string localSrcFileName, string localDestFileName);
}
