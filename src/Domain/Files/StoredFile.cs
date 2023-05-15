namespace Domain.Files;

public class StoredFile
{
    private readonly FileId _id;
    private readonly string _path;

    public StoredFile(FileId id, string path)
    {
        _id = id;
        _path = path;
    }

    public T Map<T>(IFileMapper<T> mapper) => mapper.Map(_id, _path);
}
