namespace Recipizer.Cli;

internal interface IFileSystem
{
    void Delete(string path);
    bool Exists(string path);
    void Create(string path);
    Task<string> ReadAllText(string path);
}

internal sealed class FileSystem : IFileSystem
{
    public void Delete(string path)
    {
        File.Delete(path);
    }

    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public void Create(string path)
    {
        File.Create(path);
    }

    public Task<string> ReadAllText(string path)
    {
        return File.ReadAllTextAsync(path);
    }
}