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
    [ApiExplorerSettings(GroupName = "file")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }

        [HttpPost("{id:int}/encode")]
        [ProducesResponseType(typeof(TemplateFormResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> FileEncode(int id, [FromForm] TemplateForm form)
        {
            _logger.LogDebug("Validating formId={FormId} for file={id}", form.TemplateId, id);

            if (form.TemplateFile?.Length == 0) return BadRequest("The file is empty.");
            
            using var memoryStream = new MemoryStream();
            await form.TemplateFile.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return Ok(Convert.ToBase64String(bytes));
        }

        [HttpGet("{id:int}/{templateId:int}")]
        [ProducesResponseType(typeof(TemplateFormResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TemplateFormResult> FileView(int id, int templateId)
        {
            _logger.LogDebug("Viewing templateId={templateId} for file={id}", templateId, id);
            await Task.Delay(1000);
            return new TemplateFormResult { TemplateId = templateId, FileId = id };
        }

        [HttpPost("{id:int}/upload")]
        [ProducesResponseType(typeof(TemplateFormResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<TemplateFormResult>> FileUpload(int id, [FromForm] TemplateForm form)
        {
            _logger.LogDebug("Validating formId={FormId} for file={id}", form.TemplateId, id);

            var ext = Path.GetExtension(form.TemplateFile.FileName);
            if (form.TemplateFile?.Length == 0) return BadRequest("The file is empty.");
            if (ext != ".pdf" && ext != ".docx" && ext != ".json" && ext != ".xml") return BadRequest($"The file is not supported.");

            var filename = $"{DateTime.Now:dd-MM-yyyy-HHmm}_{form.TemplateFile.FileName}";
            var filePath = Path.Combine(@"App_Data", filename);
            new FileInfo(filePath).Directory?.Create();

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                _logger.LogDebug("Saving {FileName}", form.TemplateFile.FileName);
                await form.TemplateFile.CopyToAsync(stream);
                _logger.LogDebug("File saved {filePath}.", filePath);
            }
            var result = new TemplateFormResult
            {
                FileId = id,
                TemplateId = form.TemplateId,
                FileName = filename,
                FileSize = form.TemplateFile.Length
            };
            return CreatedAtAction(nameof(FileView), new { id, form.TemplateId }, result);
        }

        [HttpPost("{id:int}/uploads")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<List<TemplateFormResult>>> FilesUpload(int id, [Required] List<IFormFile> files)
        {
            if (files?.Count == 0) return BadRequest("No file to uploaded.");

            var result = new List<TemplateFormResult>();

            foreach (var file in files)
            {
                var filePath = Path.Combine(@"App_Data", id.ToString(), file.FileName);
                new FileInfo(filePath).Directory?.Create();

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                _logger.LogDebug("Uploaded {file.FileName} and saved in {filePath}.", file.FileName, filePath);

                result.Add(new TemplateFormResult {
                    TemplateId = id,
                    FileName = file.FileName,
                    FileSize = file.Length
                });
            }
            return Ok(result);
        }

        [HttpGet("{filename}")]
        public async Task<ActionResult> FileDownload(string filename)
        {
            // validation and get the file
            var filePath = Path.Combine(@"App_Data", $"{filename}");
            if (!System.IO.File.Exists(filePath)) return new NoContentResult();

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
