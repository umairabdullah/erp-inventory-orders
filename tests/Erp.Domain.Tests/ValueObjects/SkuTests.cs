using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Tests.ValueObjects
{
    public class SkuTests
    {
        [Fact]
        public void Create_trims_and_uppercase()
        {
            Assert.Equal("LAP-15", Sku.Create("     lap-15     ").Value);
        }
        [Fact]
        public void Same_code_in_the_different_case_is_the_same_sku()
        {
            Assert.Equal(Sku.Create("lap-15"), Sku.Create("LAP-15"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ab")]
        [InlineData("a b c")]
        [InlineData("lap_15")]
        public void Create_rejects_invlid_codes(string value)
        {
            Assert.Throws<DomainException>(() => Sku.Create(value));
        }

        [Fact]
        public void Create_rejects_codes_longer_than_thirty_characters()
        {
            Assert.Throws<DomainException>(() => Sku.Create(new string('A', 31)));
        }
    }
}
