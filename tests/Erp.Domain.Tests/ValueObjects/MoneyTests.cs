using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Create_round_to_two_decimals_and_uppercase_currency()
        {
            var money = Money.Create(10.456m, "usd");
            Assert.Equal(10.46m, money.Amount);
            Assert.Equal("USD", money.Currency);
        }
        [Fact]
        public void Create_rejects_negative_amount()
        {
            Assert.Throws<DomainException>(() => Money.Create(-1m));
        }
        [Theory]
        [InlineData("")]
        [InlineData("US")]
        [InlineData("USDD")]
        public void Create_rejects_invalid_currency(string currency)
        {
            Assert.Throws<DomainException>(() => Money.Create(5m, currency));
        }

        [Fact]
        public void Two_values_with_same_amount_and_currency_are_equal()
        {
            Assert.NotEqual(Money.Create(10m, "USD"), Money.Create(10m, "EUR"));
        }

        [Fact]
        public void Add_returns_a_new_sum_and_leaves_the_original_unchanged()
        {
            var original = Money.Create(10m);
            var sum = original.Add(Money.Create(5.50m));

            Assert.Equal(15.50m, sum.Amount);
            Assert.Equal(10M, original.Amount);
        }
        [Fact]
        public void Add_reject_different_currencies()
        {
            Assert.Throws<DomainException>(() => Money.Create(10m, "USD").Add(Money.Create(5m, "EUR")));
        }

        [Fact]
        public void Multiply_scales_the_amount()
        {
            Assert.Equal(10m, Money.Create(2.50m).Multiply(4).Amount);
        }

        [Fact]
        public void Multiply_reject_negative_quantities()
        {
            Assert.Throws<DomainException>(() => Money.Create(2.50m).Multiply(-1));
        }

    }
}
