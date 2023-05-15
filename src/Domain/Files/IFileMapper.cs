namespace Domain.Files;

public interface IFileMapper<T>
{
    T Map(FileId id, string path);
}
