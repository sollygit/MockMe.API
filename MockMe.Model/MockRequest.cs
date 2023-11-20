using System;

namespace MockMe.Model
{
    public class MockRequest
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string CreatedAt { get; set; }
        public string ParticipantId { get; set; }
        public string Status { get; set; }
        public string RemoteIP { get; set; }
        public string UserAgent { get; set; }
        public string Data { get; set; }
        public string DataDetails { get; set; }
    }
}
