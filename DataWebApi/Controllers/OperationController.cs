using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using DataWebApi.Data;
using DataWebApi.Dto;
using DataWebApi.Interfaces;
using DataWebApi.Models;
using DataWebApi.Repositories;
using DataWebApi.Services;

namespace DataWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class OperationController : ControllerBase
    {
        private readonly IValueRepository _valueRepository;
        private readonly IResultRepository _resultRepository;
        private readonly IFileImporterService _fileImporterService;

        public OperationController(IValueRepository valueRepository,
            IResultRepository resultRepository,
            IFileImporterService fileImporterService)
        {
            _valueRepository = valueRepository;
            _resultRepository = resultRepository;
            _fileImporterService = fileImporterService;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ImportValuesFromFile(IFormFile file) 
        {
            var importResult = await _fileImporterService.ImportValues(file);
            var values = importResult.Values;
            if (values == null)
                return BadRequest(importResult.ErrorMessage);

            var createResult = await _resultRepository.CreateResult(values, values[0].FileName, importResult.IsOverwrite);
            if (!createResult.IsSuccessful)            
                return BadRequest(createResult.ErrorMessage);            

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(IEnumerable<ResultDto>), 200)]
        public IActionResult GetResults([FromQuery] ResultParameters resultParams)
        {
            if (!resultParams.IsValid)            
                return BadRequest("Некорректные значения для фильтрации");            

            var results = _resultRepository.GetResults(resultParams);
            return Ok(results);
        }

        [HttpGet("{fileName}")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(IEnumerable<ValueDto>), 200)] 
        public IActionResult GetValues(string fileName)
        {
            var values = _valueRepository.GetValuesByFilename(fileName);

            if (values == null)            
                return NotFound("Файл с таким именем не найден");            

            return Ok(values);
        }
    }
}
