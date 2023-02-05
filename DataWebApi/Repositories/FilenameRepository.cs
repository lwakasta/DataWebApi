using Microsoft.EntityFrameworkCore;
using DataWebApi.Data;
using DataWebApi.Interfaces;
using DataWebApi.Models;
using DataWebApi.ResultModels;

namespace DataWebApi.Repositories
{
    public class FilenameRepository : IFilenameRepository
    {
        private readonly OperationDbContext _context;

        public FilenameRepository(OperationDbContext context) 
        {
            _context = context;
        }

        public Filename? GetFilenameByName(string name)
        {
            var fileName = _context.Filenames.FirstOrDefault(x => x.Name == name); 
            return fileName;
        }

        public async Task<OperationResult> CreateFilename(Filename fileName)
        {
            _context.Filenames.Add(fileName);
            return await Save();
        }

        public async Task<OperationResult> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0
                ? new OperationResult { IsSuccessful = true }
                : new OperationResult { ErrorMessage = "Ошибка при сохранении изменений в БД" };
        }
    }
}
