using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataWebApi.Data;
using DataWebApi.Models;
using DataWebApi.Repositories;

namespace DataWebApi.Tests.Repositories
{
    public class ResultRepositoryTests
    {
        private async Task<OperationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<OperationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OperationDbContext(options);
            dbContext.Database.EnsureCreated();

            for (int i = 0; i < 5; i++)
            {
                dbContext.Results.Add(new Result
                {
                    FileName = new Filename { Name = $"file_{i}" },
                    ExecTimeTicks = 1,
                    AverageParam = i,
                    AverageTime = i,
                    MaxParam = 2,
                    MinParam = 1,
                    ParamMedian = 1.5,
                    RowsCount = 1,
                    StartTime = DateTime.Now.AddDays(i)
                });

                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [Fact]
        public async Task ResultRepository_CreateResult_ReturnsSuccess()
        {
            // Arrange
            var fileName = new Filename { Id = 11, Name = "anotherFile" };
            var values = new List<Value>
            {
                new Value
                {                    
                    Date = DateTime.Now,
                    Param = 1,
                    Time = 1,
                    FileName = fileName
                }
            };
            var dbContext = await GetDbContext();
            var resultRepository = new ResultRepository(dbContext);
            var prevCount = dbContext.Results.Count();

            // Act
            var result = resultRepository.CreateResult(values, fileName, false);

            // Assert
            Assert.True(result.Result.IsSuccessful);
            Assert.Equal(prevCount + 1, dbContext.Results.Count());
        }

        [Fact]
        public async Task ResultRepository_GetResults_ReturnsResults_StartTimeFilter()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var resultRepository = new ResultRepository(dbContext);
            var resultParams = new ResultParameters
            {
                MinStartTime = DateTime.Now.AddDays(-1),
                MaxStartTime = DateTime.Now.AddDays(4),
            };

            // Act
            var result = resultRepository.GetResults(resultParams);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task ResultRepository_GetResults_ReturnResults_AverageParamFilter()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var resultRepository = new ResultRepository(dbContext);
            var resultParams = new ResultParameters
            {
                MinAverageParam = 0,
                MaxAverageParam = 4
            };

            // Act
            var result = resultRepository.GetResults(resultParams);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task ResultRepository_GetResults_ReturnResults_AverageTimeFilter()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var resultRepository = new ResultRepository(dbContext);
            var resultParams = new ResultParameters
            {
                MinAverageTime = 0,
                MaxAverageTime = 4
            };

            // Act
            var result = resultRepository.GetResults(resultParams);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task ResultRepository_GetResults_ReturnResults_FileNameFilter()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var resultRepository = new ResultRepository(dbContext);
            var resultParams = new ResultParameters
            {
                FileName = "file_3"
            };

            // Act
            var result = resultRepository.GetResults(resultParams);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == 1);
        }

        [Fact]
        public async Task ResultRepository_UpdateResult_ReturnsSuccess()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var resultRepository = new ResultRepository(dbContext);
            var resultUpd = dbContext.Results.First();
            resultUpd.AverageTime = 10;

            // Act
            var result = resultRepository.UpdateResult(resultUpd);

            // Assert
            Assert.True(result.Result.IsSuccessful);
        }
    }
}
