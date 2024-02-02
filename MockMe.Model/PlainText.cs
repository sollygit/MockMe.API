using System.ComponentModel.DataAnnotations;

namespace MockMe.Model
{
    public class PlainText
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
