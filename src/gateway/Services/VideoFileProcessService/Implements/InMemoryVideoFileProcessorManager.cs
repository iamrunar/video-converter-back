namespace gateway.Services.VideoFileProcessService.Implements;
using gateway.Services.VideoFileProcessService.Models;

internal class InMemoryLocalVideoFileProcessorManager : IVideoFileProcessor
{
    IVideoConvertProcessor _videoConvertProcessor;
    private readonly IFileStoreService _fileStoreService;
    private readonly ILogger<InMemoryLocalVideoFileProcessorManager> _logger;
    Dictionary<string, VideoFile> _fileInfoStorage = new Dictionary<string, VideoFile>();

    Queue<string> _processQueue = new Queue<string>();
    
    Task? _convert = null;

    public InMemoryLocalVideoFileProcessorManager(IVideoConvertProcessor videoConvertProcessor, IFileStoreService fileStoreService, ILogger<InMemoryLocalVideoFileProcessorManager> logger)
    {
        _videoConvertProcessor = videoConvertProcessor;
        this._fileStoreService = fileStoreService;
        this._logger = logger;
    }
    
    public  Task<string?> GetFileUrlByProcIdAsync(string procId)
    {
         _fileInfoStorage.TryGetValue(procId, out VideoFile? videoFile);
         return Task.FromResult(videoFile?.FileDestUrl);
    }

    public string Enqueue(string srcFilePath)
    {
        _logger.LogDebug("Start process enqueue {0}",srcFilePath);

        var processId = GenerateVideoFileProcessId();
        var videoDestFile = GenerateDestVideoFileUrl();
        var videoFile = new VideoFile(processId, srcFilePath,videoDestFile);
        _fileInfoStorage.Add(processId, videoFile);

        _logger.LogDebug("Enqueue {0} {1} {2}", processId, srcFilePath, videoDestFile);
        _processQueue.Enqueue(processId);

        StartIfIsNot();

        _logger.LogDebug("Finish process enqueue {0}",srcFilePath);

        return processId;

        void StartIfIsNot()
        {
            // todo: synch
            var task = Volatile.Read(ref _convert);
            if (task!=null) _logger.LogDebug("Task will not be started.");
            if (task==null)
            {
                _logger.LogDebug("Start queue processor {0}", srcFilePath);
                task = Task.Factory.StartNew(QueueProcessor, TaskCreationOptions.LongRunning);
                Volatile.Write(ref _convert, task);
            }
        }
    }

    internal async Task QueueProcessor()
    {
        while (_processQueue.TryDequeue(out var processId))
        {
            var vr = _fileInfoStorage[processId];
            //once per time
            await ConvertAsync(vr.FileSrcUrl, vr.FileDestUrl, true);
        }

        // todo: synch
        Volatile.Write(ref _convert, null);
        _logger.LogDebug($"QueueProcessor finished because there are not any items in {nameof(_processQueue)}");
    }

    internal async Task ConvertAsync(string srcFile, string destFile, bool removeSrcFileWhenComplited)
    {
        _logger.LogDebug("Starting {0}.", srcFile);
        await _videoConvertProcessor.ConvertAsync(srcFile, destFile);
        _logger.LogDebug("Complited {0}.", srcFile);
        if (removeSrcFileWhenComplited)
        {
            await _fileStoreService.RemoveFileAsync(srcFile);
            _logger.LogDebug("Removed {0}", srcFile);
        }
    }

    internal string GenerateDestVideoFileUrl()
    {
        return Path.GetTempFileName()+".mp4";
    }

    internal string GenerateVideoFileProcessId()
    {
        return Guid.NewGuid().ToString("n");
    }

    public async Task DeleteAsync(string procId)
    {
        if (!_fileInfoStorage.ContainsKey(procId))
        {
            throw new FileNotFoundException($"ProcId={procId} was not found");
        }

        var destFilePath = _fileInfoStorage[procId].FileDestUrl;
        await _fileStoreService.RemoveFileAsync(destFilePath);

        _fileInfoStorage.Remove(procId);
    }
}
