namespace MockMe.API.ViewModels
{
    public class AssetTradeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Expiration { get; set; }
        public decimal Amount { get; set; }
        public int Direction { get; set; }
        public int Payout { get; set; }
    }
}
