namespace gateway.Services;
public interface IVideoFileProcessor
{
    Task<string?> GetFileUrlByProcIdAsync(string procId);
    string Enqueue(string srcFilePath);
    Task DeleteAsync(string procId);
}
