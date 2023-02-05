using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DataWebApi.Data;
using DataWebApi.Dto;
using DataWebApi.Models;
using DataWebApi.Interfaces;
using DataWebApi.ResultModels;
using System.Runtime.CompilerServices;
using System.IO.Pipelines;

namespace DataWebApi.Repositories
{
    public class ValueRepository : IValueRepository
    {
        private readonly OperationDbContext _context;

        public ValueRepository(OperationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> CreateValues(List<Value> values)
        {
            // в списке values могут быть как перезаписанные значения, так и новые, поэтому используется Update
            _context.Values.UpdateRange(values);
            return await Save();           
        }        

        public List<Value> GetValuesByFileId(int fileId)
        {
            return _context.Values.Where(x => x.FileId == fileId).ToList();
        }

        public IEnumerable<ValueDto>? GetValuesByFilename(string fileName)
        {
            var fileId = _context.Filenames.FirstOrDefault(x => x.Name == fileName)?.Id;

            if (fileId == null)            
                return null;            

            var values = from v in _context.Values
                         join f in _context.Filenames on v.FileId equals f.Id
                         where f.Id == fileId
                         select new ValueDto
                         {
                             Date = v.Date,
                             Time = v.Time,
                             Param = v.Param
                         };

            return values.ToList();
        }

        public async Task<OperationResult> DeleteValue(Value value)
        {
            _context.Remove(value);
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
