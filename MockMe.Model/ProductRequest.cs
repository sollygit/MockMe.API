using System;

namespace MockMe.Model
{
    public class ProductRequest
    {
        public Guid Id { get; set; }
        public Guid ParticipantId { get; set; }
        public int ReferenceNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}
