using System.ComponentModel;

namespace MockMe.Common
{
    public enum CurrencyPair
    {
        [Description("EUR/USD")]
        EUR_USD = 1,
        [Description("JPY/USD")]
        JPY_USD = 2,
        [Description("GBP/USD")]
        GBP_USD = 3
    }
}
