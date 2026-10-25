using Erp.Domain.Exceptions;

namespace Erp.Domain.ValueObjects
{
    public sealed record class Money
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new DomainException("Amount can't be negative.");

            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
                throw new DomainException("currency must be a 3-letter code");

            return new Money(Math.Round(amount, 2), currency.Trim().ToUpperInvariant());
        }

        public static Money Zero(string currency = "USD") => Create(0, currency);

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return Create(Amount + other.Amount, Currency);
        }

        public Money Multiply(int quantity)
        {
            if(quantity < 0)
                throw new DomainException("Quantity can't be negative.");

            return Create(Amount * quantity, Currency);

        }

        private void EnsureSameCurrency(Money other)
        {
            if(Currency != other.Currency)
            {
                throw new DomainException("Cannot combine different currencies.");
            }
        }
    }
}
