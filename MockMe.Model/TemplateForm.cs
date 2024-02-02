using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MockMe.Model
{
    public class TemplateForm
    {
        [Required]
        public int TemplateId { get; set; }
        
        [Required]
        public IFormFile TemplateFile { get; set; }
    }
}
