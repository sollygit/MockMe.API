using System;

namespace MockMe.Model
{
    public class AssetTrade
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Asset Asset { get; set; }
        public int Expiration { get; set; }
        public decimal Amount { get; set; }
        public int Direction { get; set; }
        public int Payout { get; set; }
    }
}
