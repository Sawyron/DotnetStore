using Domain.Categories;
using Domain.Files;

namespace Application.Images;

public interface IFileRepository
{
    Task AddAsync(StoredFile file);
    Task<StoredFile?> FindByIdAsync(FileId id);
    Task<List<StoredFile>> FindCategoryFiles(CategoryId categoryId);
    Task<bool> DeleteAsync(FileId fileId);
}
