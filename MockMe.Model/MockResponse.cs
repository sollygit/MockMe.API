using System;

namespace MockMe.Model
{
    public class MockResponse
    {
        public Guid Id { get; set; }
        public string AccessToken { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }
}
