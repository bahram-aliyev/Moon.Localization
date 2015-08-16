using System.Globalization;
using Xunit;

namespace Moon.Localization.Tests
{
    public class CurrencyTests
    {
        [Theory]
        [InlineData("EUR", "€"), InlineData("CZK", "Kč"), InlineData("USD", "$")]
        [InlineData("JPY", "¥"), InlineData("GBP", "£"), InlineData("CNY", "¥")]
        public void GetSymbol_ForKnownCurrency_ResultIsSymbol(string currency, string symbol)
        {
            Assert.Equal(symbol, Currency.GetSymbol(currency));
        }

        [Fact]
        public void GetSymbol_ForUnknownCurrency_ResultIsCode()
        {
            Assert.Equal("UNK", Currency.GetSymbol("UNK"));
        }

        [Theory]
        [InlineData("USD", 10, "$10"), InlineData("USD", -10, "($10)"), InlineData("EUR", -10.30, "-10.3 €")]
        public void Format_FroKnownCurrency_ResultIsFromattedAsCurrency(string currency, double value, string result)
        {
            Assert.Equal(result, Currency.Format(value, currency, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Format_ForUnknownCurrency_ResultIsCode()
        {
            Assert.Equal("15.6 UNK", Currency.Format(15.60, "UNK", CultureInfo.InvariantCulture));
        }
    }
}