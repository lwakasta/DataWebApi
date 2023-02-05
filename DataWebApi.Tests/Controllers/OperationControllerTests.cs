using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataWebApi.Controllers;
using DataWebApi.Dto;
using DataWebApi.Interfaces;
using DataWebApi.Models;
using DataWebApi.ResultModels;
using DataWebApi.Services;

namespace DataWebApi.Tests.Controllers
{
    public class OperationControllerTests
    {
        private readonly IValueRepository _valueRepository;
        private readonly IResultRepository _resultRepository;
        private readonly IFileImporterService _fileImporterService;

        public OperationControllerTests()
        {
            _valueRepository = A.Fake<IValueRepository>();
            _resultRepository = A.Fake<IResultRepository>();
            _fileImporterService = A.Fake<IFileImporterService>();
        }

        [Fact]
        public async Task OperationController_ImportValuesFromCsv_ReturnsOk()
        {
            // Arrange
            var fakeFile = A.Fake<IFormFile>();
            var fakeFilename = A.Fake<Filename>();
            var fakeValues = new List<Value>() { new Value { FileName = fakeFilename } };            
            var fakeImportResult = new ImportResult { Values = fakeValues };
            var fakeOperationResult = new OperationResult { IsSuccessful = true };
            A.CallTo(() => _fileImporterService.ImportValues(fakeFile)).Returns(fakeImportResult);
            A.CallTo(() => _resultRepository.CreateResult(fakeValues, fakeFilename, false)).Returns(fakeOperationResult);
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = await controller.ImportValuesFromFile(fakeFile);
            var okResult = result as OkResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult?.StatusCode);
        }

        [Fact]
        public async Task OperationController_ImportValuesFromCsv_ReturnsBadRequest_ImportFails()
        {
            // Arrange
            var fakeFile = A.Fake<IFormFile>();
            var fakeFilename = A.Fake<Filename>();
            var fakeValues = new List<Value>() { new Value { FileName = fakeFilename } };
            var errorMessage = "Import failed";
            var fakeImportResult = new ImportResult { Values = null, ErrorMessage = errorMessage };
            var fakeOperationResult = new OperationResult { IsSuccessful = true };
            A.CallTo(() => _fileImporterService.ImportValues(fakeFile)).Returns(fakeImportResult);
            A.CallTo(() => _resultRepository.CreateResult(fakeValues, fakeFilename, false)).Returns(fakeOperationResult);
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = await controller.ImportValuesFromFile(fakeFile);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult?.StatusCode);
            Assert.Equal(errorMessage, badResult?.Value);
        }

        [Fact]
        public async Task OperationController_ImportValuesFromCsv_ReturnsBadRequest_CreateResultFails()
        {
            // Arrange
            var fakeFile = A.Fake<IFormFile>();
            var fakeFilename = A.Fake<Filename>();
            var fakeValues = new List<Value>() { new Value { FileName = fakeFilename } };
            var fakeImportResult = new ImportResult { Values = fakeValues };
            var errorMessage = "Create failed";
            var fakeOperationResult = new OperationResult { ErrorMessage = errorMessage };
            A.CallTo(() => _fileImporterService.ImportValues(fakeFile)).Returns(fakeImportResult);
            A.CallTo(() => _resultRepository.CreateResult(fakeValues, fakeFilename, false)).Returns(fakeOperationResult);
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = await controller.ImportValuesFromFile(fakeFile);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult?.StatusCode);
            Assert.Equal(errorMessage, badResult?.Value);
        }

        [Fact]
        public void OperationController_GetResults_ReturnsOk()
        {
            // Arrange
            var fakeResultParams = A.Fake<ResultParameters>();
            var fakeResults = A.Fake<IEnumerable<ResultDto>>();
            A.CallTo(() => _resultRepository.GetResults(fakeResultParams)).Returns(fakeResults);
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = controller.GetResults(fakeResultParams);
            var okResult = result as OkObjectResult;
            var resultValue = okResult?.Value as IEnumerable<ResultDto>;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult?.StatusCode);
            Assert.NotNull(resultValue);
            Assert.Equal(fakeResults, resultValue);
        }

        [Fact]
        public void OperationController_GetResults_ReturnsBadRequest_MaxAverageParamLessThanMin()
        {
            // Arrange
            var fakeResultParams = new ResultParameters { 
                MinAverageParam = 1,
                MaxAverageParam = 0 
            };
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = controller.GetResults(fakeResultParams);
            var badResult = result as BadRequestObjectResult;
            var resultValue = badResult?.Value as string;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult?.StatusCode);
            Assert.Equal("Некорректные значения для фильтрации", resultValue);
        }

        [Fact]
        public void OperationController_GetResults_ReturnsBadRequest_MaxAverageTimeLessThanMin()
        {
            // Arrange
            var fakeResultParams = new ResultParameters
            {
                MinAverageTime = 1,
                MaxAverageTime = 0
            };
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = controller.GetResults(fakeResultParams);
            var badResult = result as BadRequestObjectResult;
            var resultValue = badResult?.Value as string;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult?.StatusCode);
            Assert.Equal("Некорректные значения для фильтрации", resultValue);
        }

        [Fact]
        public void OperationController_GetResults_ReturnsBadRequest_MaxStartTimeLessThanMin()
        {
            // Arrange
            var fakeResultParams = new ResultParameters
            {
                MinStartTime = DateTime.Now.AddMinutes(1),
                MaxStartTime = DateTime.Now
            };
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = controller.GetResults(fakeResultParams);
            var badResult = result as BadRequestObjectResult;
            var resultValue = badResult?.Value as string;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult?.StatusCode);
            Assert.Equal("Некорректные значения для фильтрации", resultValue);
        }

        [Fact]
        public void OperationController_GetValues_ReturnsOk()
        {
            // Arrange
            var fakeFilename = "xxx.csv";
            var fakeValues = A.Fake<IEnumerable<ValueDto>>();
            A.CallTo(() => _valueRepository.GetValuesByFilename(fakeFilename)).Returns(fakeValues);
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = controller.GetValues(fakeFilename);
            var okResult = result as OkObjectResult;
            var resultValue = okResult?.Value as IEnumerable<ValueDto>;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult?.StatusCode);
            Assert.NotNull(resultValue);
            Assert.Equal(fakeValues, resultValue);
        }

        [Fact]
        public void OperationController_GetValues_ReturnsNotFound()
        {
            // Arrange
            var fakeFilename = "xxx.csv";
            var fakeValues = A.Fake<IEnumerable<ValueDto>>();
            A.CallTo(() => _valueRepository.GetValuesByFilename(fakeFilename)).Returns(null);
            var controller = new OperationController(_valueRepository, _resultRepository, _fileImporterService);

            // Act
            var result = controller.GetValues(fakeFilename);
            var notFoundResult = result as NotFoundObjectResult;
            var resultValue = notFoundResult?.Value as string;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult?.StatusCode);
            Assert.Equal("Файл с таким именем не найден", resultValue);
        }
    }
}
