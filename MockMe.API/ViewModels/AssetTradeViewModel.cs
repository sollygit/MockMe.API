namespace MockMe.API.ViewModels
{
    public class AssetTradeViewModel
    {
        public AssetViewModel Asset { get; set; }
        public int Expiration { get; set; }
        public decimal Amount { get; set; }
        public int Direction { get; set; }
        public int Payout { get; set; }
    }
}
