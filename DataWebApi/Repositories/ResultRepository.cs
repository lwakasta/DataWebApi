using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataWebApi.Data;
using DataWebApi.Dto;
using DataWebApi.Interfaces;
using DataWebApi.Models;
using DataWebApi.ResultModels;

namespace DataWebApi.Repositories
{
    public class ResultRepository : IResultRepository
    {
        private readonly OperationDbContext _context;

        public ResultRepository(OperationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> CreateResult(List<Value> values, Filename fileName, bool isOverwrite)
        {            
            values.Sort((x, y) => x.Param.CompareTo(y.Param));
            var result = new Result
            {
                FileId = fileName.Id,
                FileName = fileName,
                ExecTimeTicks = values.Max(x => x.Date).Subtract(values.Min(x => x.Date)).Ticks,
                StartTime = values.Min(x => x.Date),
                AverageTime = Convert.ToInt32(values.Average(x => x.Time)),
                AverageParam = values.Average(x => x.Param),
                ParamMedian = values[values.Count / 2].Param,
                MaxParam = values[values.Count - 1].Param,
                MinParam = values[0].Param,
                RowsCount = values.Count
            };

            if (isOverwrite)
                return await UpdateResult(result);

            _context.Results.Add(result);
            return await Save();
        }

        public IEnumerable<ResultDto> GetResults(ResultParameters resultParams)
        {
            var results = _context.Results.Include(f => f.FileName).AsQueryable();
            if (!resultParams.FileName.IsNullOrEmpty())
            {
                results = results.Where(x => x.FileName.Name.StartsWith(resultParams.FileName!));
            }
            var resultsDto = results.Where(x => x.StartTime >= resultParams.MinStartTime && x.StartTime <= resultParams.MaxStartTime &&
                                          x.AverageParam >= resultParams.MinAverageParam && x.AverageParam <= resultParams.MaxAverageParam &&
                                          x.AverageTime >= resultParams.MinAverageTime && x.AverageTime <= resultParams.MaxAverageTime)
                                    .Select(x => new ResultDto
                                    {
                                        FileName = x.FileName.Name,
                                        ExecTimeTicks = x.ExecTimeTicks,
                                        StartTime = x.StartTime,
                                        AverageTime = x.AverageTime,
                                        AverageParam = x.AverageParam,
                                        ParamMedian = x.ParamMedian,
                                        MaxParam = x.MaxParam,
                                        MinParam = x.MinParam,
                                        RowsCount = x.RowsCount
                                    });

            return resultsDto.ToList();
        }

        public async Task<OperationResult> UpdateResult(Result result)
        {
            _context.Results.Update(result);
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
