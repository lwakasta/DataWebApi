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
    public class FilenameRepositoryTests
    {
        private async Task<OperationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<OperationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid()
                .ToString()).Options;
            var dbContext = new OperationDbContext(options);
            dbContext.Database.EnsureCreated();

            for (int i = 0; i < 5; i++)
            {
                dbContext.Filenames.Add(new Filename { Name = i.ToString() });
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [Fact]
        public async Task FilenameRepository_GetFilename_ReturnsFilename()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var filenameRepository = new FilenameRepository(dbContext);            

            // Act
            var result = filenameRepository.GetFilenameByName("1");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task FilenameRepository_CreateFilename_ReturnsSuccess()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var filenameRepository = new FilenameRepository(dbContext);
            var filename = new Filename { Name = "newFile" };

            // Act
            var result = filenameRepository.CreateFilename(filename);

            // Assert
            Assert.True(result.Result.IsSuccessful);
        }
    }
}
