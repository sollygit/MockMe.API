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
        GBP_USD = 3,
        [Description("AUD/USD")]
        AUD_USD = 4,
        [Description("USD/CAD")]
        USD_CAD = 5,
        [Description("USD/CHF")]
        USD_CHF = 6,
        [Description("NZD/USD")]
        NZD_USD = 7,
        [Description("EUR/GBP")]
        EUR_GBP = 8,
        [Description("EUR/JPY")]
        EUR_JPY = 9,
        [Description("GBP/JPY")]
        GBP_JPY = 10
    }
}
