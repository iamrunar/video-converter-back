using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using gateway.Services;

namespace gateway.Controllers;

[ApiController]
[Route("[controller]/v1")]
public class ConvertController: ControllerBase
{
    IVideoFileProcessor _videoFileProcessService;
    IFileStoreService _fileStoreService;
    ILogger<ConvertController> _logger;

    public ConvertController(ILogger<ConvertController> logger, IVideoFileProcessor videoFileProcessService, IFileStoreService fileStoreService)
    {
        _logger = logger ;
        _videoFileProcessService = videoFileProcessService ?? throw new ArgumentNullException(nameof(videoFileProcessService));
        _fileStoreService = fileStoreService ?? throw new ArgumentNullException(nameof(fileStoreService));
    }

    [HttpPost("process")]
    public async Task<string> ProcessAsync(IFormFile file)
    {
        _logger.LogDebug("Process command ProcessAsync. Args: file={0}", file.FileName);
        //https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0
        string srcFilePath = await _fileStoreService.StoreFileAsync(file.OpenReadStream(), true);
        string procId = _videoFileProcessService.Enqueue(srcFilePath);
        _logger.LogDebug("Process command finished with result {0}", procId);
        return procId;
    }

    [HttpGet("{procId}/file")]
    public async Task<IActionResult> GetFileUrlAsync(string procId)
    {
        string? fileUrl = await _videoFileProcessService.GetFileUrlByProcIdAsync(procId);
        if (fileUrl==null)
        {
            return NotFound($"The File by procId={procId} wasn't found.");
        }
        return Content(fileUrl);
    }

    [HttpDelete("/{procId}/file")]
    public async Task<IActionResult> ForgetForProcIdAsync(string procId)
    {
        try
        {
            await _videoFileProcessService.DeleteAsync(procId);
        }
        catch (FileNotFoundException)
        {
            return NotFound($"The file by procId={procId} was not found.");
        }

        return this.Ok(procId);
    }
}