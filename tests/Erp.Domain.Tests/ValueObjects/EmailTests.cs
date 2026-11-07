using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_trims_and_lowercase()
        {
            Assert.Equal("timmy@example.com", Email.Create("   Timmy@example.com    ").Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        [InlineData("a@b")]
        [InlineData("a b@c.com")]
        public void Create_rejects_invlid_addresses(string value)
        {
            Assert.Throws<DomainException>(() => Email.Create(value).Value);
        }

        [Fact]
        public void Create_rejects_addresses_longer_than_256_characters()
        {
            var tooLong = new string('a', 251) + "@x.com";
            var ex = Assert.Throws<DomainException>(() => Email.Create(tooLong));
            Assert.Contains("256", ex.Message);
        }

    }
}
