using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using MockMe.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace MockMe.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id:int}/form/{formId:int}")]
        [ProducesResponseType(typeof(SubmissionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<SubmissionResult> ViewForm(int id, int formId)
        {
            _logger.LogInformation($"viewing the form#{formId} for Student ID={id}");
            await Task.Delay(1000);
            return new SubmissionResult { FormId = formId, StudentId = id };
        }

        [HttpPost("{id:int}/form")]
        [ProducesResponseType(typeof(SubmissionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<SubmissionResult>> SubmitForm(int id, [FromForm] StudentForm form)
        {
            _logger.LogInformation($"Validating the form#{form.FormId} for Student ID={id}");

            if (form.Courses?.Length == 0) return BadRequest("Please enter at least one course.");
            if (form.StudentFile?.Length == 0) return BadRequest("The file is empty.");
            if (Path.GetExtension(form.StudentFile.FileName) != ".pdf") return BadRequest($"The file is not a PDF file.");

            var filename = $"{DateTime.Now:yyyyMMdd-HHmm}-{form.StudentFile.FileName}";
            var filePath = Path.Combine(@"App_Data", filename);
            new FileInfo(filePath).Directory?.Create();

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                _logger.LogInformation($"Saving file [{form.StudentFile.FileName}]");
                await form.StudentFile.CopyToAsync(stream);
                _logger.LogInformation($"\t The uploaded file is saved as [{filePath}].");
            }
            var result = new SubmissionResult {
                StudentId = id, FormId = form.FormId,
                FileName = filename, FileSize = form.StudentFile.Length 
            };
            return CreatedAtAction(nameof(ViewForm), new { id, form.FormId }, result);
        }

        [HttpPost("{id:int}/forms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<List<SubmissionResult>>> SubmitForms(int id, [Required] List<IFormFile> files)
        {
            if (files?.Count == 0) return BadRequest("No file is uploaded.");
            
            var result = new List<SubmissionResult>();
            foreach (var file in files)
            {
                var filePath = Path.Combine(@"App_Data", id.ToString(), @"Files", file.FileName);
                new FileInfo(filePath).Directory?.Create();

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                _logger.LogInformation($"The uploaded file [{file.FileName}] is saved as [{filePath}].");

                result.Add(new SubmissionResult { 
                    StudentId = id, FileName = file.FileName, FileSize = file.Length });
            }
            return Ok(result);
        }

        [HttpGet("files/{id}")]
        public async Task<ActionResult> DownloadFile(string id)
        {
            // validation and get the file
            var filePath = Path.Combine(@"App_Data", $"{id}.pdf");
            if (!System.IO.File.Exists(filePath))
            {
                return new NoContentResult();
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }
    }
}
