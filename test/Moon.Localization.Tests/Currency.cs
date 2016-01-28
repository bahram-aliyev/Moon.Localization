using System.Globalization;
using FluentAssertions;
using Moon.Testing;
using Xunit;

namespace Moon.Localization.Tests
{
    public class CurrencyTests : TestSetup
    {
        string result;

        [Theory]
        [InlineData("EUR", "€"), InlineData("CZK", "Kč"), InlineData("USD", "$")]
        [InlineData("JPY", "¥"), InlineData("GBP", "£"), InlineData("CNY", "¥")]
        public void GettingSymbolForKnownCurrency(string currency, string symbol)
        {
            "When I get a symbol for currency"
                .x(() => result = Currency.GetSymbol(currency));

            "The it should return expected symbol"
                .x(() =>
                {
                    result.Should().Be(symbol);
                });
        }

        [Fact]
        public void GettingSymbolForUnknownCurrency()
        {
            "When I get a symbol for unknown currency"
                .x(() => result = Currency.GetSymbol("UNK"));

            "The it should return currency code"
                .x(() =>
                {
                    result.Should().Be("UNK");
                });
        }

        [Theory]
        [InlineData("USD", 10, "$10"), InlineData("USD", -10, "($10)"), InlineData("EUR", -10.30, "-10.3 €")]
        public void FormattingNumberFroKnownCurrency(string currency, double value, string formatted)
        {
            "When I format the value as currency"
                .x(() => result = Currency.Format(value, currency, CultureInfo.InvariantCulture));

            "Then it should return formatted text"
                .x(() =>
                {
                    result.Should().Be(formatted);
                });
        }
    }
}