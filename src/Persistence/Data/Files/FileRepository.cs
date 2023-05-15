using Application.Images;
using Domain.Categories;
using Domain.Files;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Images;
using Persistence.Data.Products;

namespace Persistence.Data.Files
{
    internal class FileRepository : IFileRepository
    {
        private readonly ApplicationContext _context;
        private readonly IFileMapper<FileData> _dataMapper;

        public FileRepository(ApplicationContext context, IFileMapper<FileData> dataMapper)
        {
            _context = context;
            _dataMapper = dataMapper;
        }

        public async Task AddAsync(StoredFile file)
        {
            await _context.Set<FileData>().AddAsync(file.Map(_dataMapper));
        }

        public async Task<bool> DeleteAsync(FileId fileId)
        {
            FileData? file = await _context.Set<FileData>()
                .FindAsync(fileId);
            if (file is null)
            {
                return false;
            }
            _context.Set<FileData>().Remove(file);
            return true;
        }

        public async Task<StoredFile?> FindByIdAsync(FileId id)
        {
            FileData? file = await _context.Set<FileData>()
                .AsNoTracking()
                .FirstOrDefaultAsync(image => image.Id == id);
            return file?.ToDomainModel();
        }

        public async Task<List<StoredFile>> FindCategoryFiles(CategoryId categoryId)
        {
            List<FileData> files = await _context.Set<ProductData>()
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Photo)
                .Select(p => p.Photo)
                .ToListAsync();
            return files.Select(f => f.ToDomainModel()).ToList();
        }
    }
}
