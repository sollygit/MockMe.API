using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using MockMe.Model;
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

        [HttpPost("{id:int}/upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<TemplateFormResult>> FileUpload(int id, [FromForm] TemplateForm form)
        {
            _logger.LogDebug("Validating formId={FormId} for file={id}", form.TemplateId, id);

            var ext = Path.GetExtension(form.TemplateFile.FileName);
            if (form.TemplateFile?.Length == 0) return BadRequest("The file is empty.");
            if (ext != ".pdf" && ext != ".docx" && ext != ".json" && ext != ".xml") return BadRequest($"The file is not supported.");

            var filePath = Path.Combine(@"App_Data", form.TemplateFile.FileName);
            new FileInfo(filePath).Directory?.Create();

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.TemplateFile.CopyToAsync(stream);
                _logger.LogDebug("File uploaded to {filePath}.", filePath);
            }
            var result = new TemplateFormResult
            {
                FileId = id,
                TemplateId = form.TemplateId,
                FileName = form.TemplateFile.FileName,
                FileSize = form.TemplateFile.Length
            };
            return CreatedAtAction(nameof(FileUpload), new { id, form.TemplateId }, result);
        }

        [HttpPost("{id:int}/uploads")]
        [ProducesResponseType(StatusCodes.Status201Created)]
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
                _logger.LogDebug("File uploaded to {filePath}.", filePath);

                result.Add(new TemplateFormResult {
                    FileId = -1,
                    TemplateId = id,
                    FileName = file.FileName,
                    FileSize = file.Length
                });
            }
            return CreatedAtAction(nameof(FilesUpload), new { id, templateId = id }, result);
        }

        [HttpGet("{filename}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
