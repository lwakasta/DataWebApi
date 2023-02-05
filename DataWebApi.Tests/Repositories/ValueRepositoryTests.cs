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
    public class ValueRepositoryTests
    {
        private async Task<OperationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<OperationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid()
                .ToString()).Options;
            var dbContext = new OperationDbContext(options);
            dbContext.Database.EnsureCreated();

            var fileName = new Filename { Name = "file" };
            while (await dbContext.Values.CountAsync() < 3)
            {
                dbContext.Values.Add(new Value 
                { 
                    Date = DateTime.Now, 
                    Param = 1, 
                    Time = 1, 
                    FileName = fileName
                });

                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [Fact]
        public async Task ValueRepository_CreateValues_ReturnsSuccess()
        {
            // Arrange
            var values = new List<Value>
            {
                new Value
                {
                    Date = DateTime.Now,
                    Param = 1,
                    Time = 1,
                    FileName = new Filename
                    {
                        Name = "anotherFile"
                    }
                }   
            };            
            var dbContext = await GetDbContext();
            var valueRepository = new ValueRepository(dbContext);
            var prevCount = dbContext.Values.Count();

            // Act
            var result = valueRepository.CreateValues(values);

            // Assert
            Assert.True(result.Result.IsSuccessful);
            Assert.Equal(prevCount + 1, dbContext.Values.Count());
        }

        [Fact]
        public async Task ValueRepository_GetValuesByFileId_ReturnsValues()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var valueRepository = new ValueRepository(dbContext);

            // Act
            var result = valueRepository.GetValuesByFileId(1);

            // Assert
            Assert.Equal(dbContext.Values.Count(), result.Count);
        }

        [Fact]
        public async Task ValueRepository_GetValuesByFilename_ReturnsValues()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var valueRepository = new ValueRepository(dbContext);

            // Act
            var result = valueRepository.GetValuesByFilename("file");

            // Assert
            Assert.Equal(dbContext.Values.Count(), result?.Count());
        }

        [Fact]
        public async Task ValueRepository_DeleteValue_ReturnsSuccess()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var valueRepository = new ValueRepository(dbContext);
            var value = dbContext.Values.First();
            var prevCount = dbContext.Values.Count();

            // Act
            var result = valueRepository.DeleteValue(value);

            // Assert
            Assert.True(result.Result.IsSuccessful);
            Assert.Equal(prevCount - 1, dbContext.Values.Count());
        }
    }
}
