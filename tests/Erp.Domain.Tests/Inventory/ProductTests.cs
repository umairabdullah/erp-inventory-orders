using Erp.Domain.Exceptions;
using Erp.Domain.Inventory;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Tests.Inventory
{
    public class ProductTests
    {
        private static Product NewProduct(int stock = 10, int reorderLevel = 3) => 
            Product.Create("Laptop-15", null, Sku.Create("LAP-15"), Money.Create(750m), stock, reorderLevel, Guid.NewGuid());

        [Fact]
        public void Crete_rejects_negative_initial_stock()
        {
            Assert.Throws<DomainException>(() => NewProduct(stock: -1));
        }

        [Fact]
        public void Create_requires_a_category()
        {
            Assert.Throws<DomainException>(() => Product.Create("Laptop", null, Sku.Create("LAP-15"), Money.Create(1m), 1, 1, Guid.Empty));
        }

        [Fact]
        public void Addstock_increases_quantitiy()
        {
            var product = NewProduct(stock: 10);
            product.AddStock(5);
            Assert.Equal(15, product.QuantityInStock);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Addstock_rejects_non_positive_quantity(int quantity)
        {
            Assert.Throws<DomainException>(() => NewProduct().AddStock(quantity));
        }

        [Fact]
        public void Remove_stock_reduces_quantity()
        {
            var product = NewProduct(stock: 10);
            product.RemoveStock(4);
            Assert.Equal(6, product.QuantityInStock);
        }

        [Fact]
        public void Remove_stock_can_remove_all_remaining_stock()
        {
            var product = NewProduct(stock: 10);
            product.RemoveStock(10);
            Assert.Equal(0, product.QuantityInStock);
        }

        [Fact]
        public void Remove_stock_can_go_below_zero_and_leaves_stock_unchanged()
        {
            var product = NewProduct(stock: 10);
            var ex = Assert.Throws<DomainException>(() => product.RemoveStock(11));

            Assert.Contains("Available: 10", ex.Message);
            Assert.Equal(10, product.QuantityInStock);
        }

        [Theory]
        [InlineData(3, 3, true)]
        [InlineData(2, 3, true)]
        [InlineData(4, 3, false)]
        public void Is_low_Stock_is_true_at_or_below_the_reorderLevel(int stock, int reorderLevel, bool expected)
        {
            Assert.Equal(expected, NewProduct(stock, reorderLevel).IsLowStock);
        }

        [Fact]
        public void Updatedetails_changes_the_details_but_not_stock_or_sku()
        {
            var product = NewProduct(stock: 10);
            var newCategory = Guid.NewGuid();

            product.UpdateDetails("Laptop 15 pro", "New", Money.Create(900m), 5, newCategory);

            Assert.Equal("Laptop 15 pro", product.Name);
            Assert.Equal(900m, product.Price.Amount);
            Assert.Equal(newCategory, product.CategoryId);
            Assert.Equal(10, product.QuantityInStock);
            Assert.Equal("LAP-15", product.Sku.Value);
        }

        [Fact]
        public void Deactivate_and_activate_toggle_the_status()
        {
            var product = NewProduct();
            product.Deactivate();

            Assert.False(product.IsActive);

            product.Activate();
            Assert.True(product.IsActive);
        }
    }
}
