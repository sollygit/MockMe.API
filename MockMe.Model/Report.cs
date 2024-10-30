using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MockMe.Model
{
    public class Report
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string SourceShort => Uri.TryCreate(Source, UriKind.Absolute, out var sourceUri) ?  sourceUri.Authority : Source;
        public int PrepTime { get; set; }
        public int WaitTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public string Comments { get; set; }
        public IList<Review> Reviews { get; set; } = new List<Review>();
        public string Instructions { get; set; }
        public string[] Ingredients { get; set; }
        public string[] Tags { get; set; }
        public Uri CardImageUrl => new Uri($"images/cards/{Name}.png", UriKind.Relative);
        public Uri BannerImageUrl => new Uri($"images/banners/{Name} Banner.png", UriKind.Relative);
    }

    public class Review
    {
        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Text must be no more than 50 characters.")]
        public string Text { get; set; }
    }
}
