using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MockMe.Model
{
    public class StudentForm
    {
        [Required] public int FormId { get; set; }
        [Required] public string[] Courses { get; set; }
        [Required] public IFormFile StudentFile { get; set; }
    }
}
