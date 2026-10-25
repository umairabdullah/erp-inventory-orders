using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Domain.Inventory
{
    public sealed class Product : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public Sku Sku { get; private set; } = null!;
        public Money Price { get; private set; } = null!;
        public int QuantityInStock { get; private set; }
        public int ReorderLevel { get; private set; }
        public bool IsActive { get; private set; } = true;
        public Guid CategoryId { get; private set; }

        public bool IsLowStock => QuantityInStock <= ReorderLevel;

        private Product()
        {
            
        }

        public static Product Create(string name, string? description, Sku sku, Money price, int initialStock, int reorderLevel, Guid categoryId)
        {
            if (initialStock < 0)
                throw new DomainException("Initial stock cannot be negative.");

            var product = new Product()
            {
                Sku = sku,
                QuantityInStock = initialStock
            };

            product.ApplyDetails(name, description, price, reorderLevel, categoryId);
            return product;
            
        }

        public void UpdateDetails(string name, string? description, Money price, int reorderLevel, Guid categoryId)
        {
            ApplyDetails(name, description, price, reorderLevel, categoryId);
            MarkUpdated();
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to add must be greater than zero.");
            QuantityInStock += quantity;
            MarkUpdated();
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to remove must be greater than zero.");

            if (quantity > QuantityInStock)
                throw new DomainException($"Insufficent stock. Available {QuantityInStock}.");

            QuantityInStock -= quantity;
            MarkUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkUpdated();
        }

        private void ApplyDetails(string name, string? description, Money price, int reorderLevel, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name is required.");

            if(name.Trim().Length > 200)
                throw new DomainException("Product name cannot exceed 200 characters.");

            if(reorderLevel < 0)
                throw new DomainException("Reorder level cannot be negative.");

            if (categoryId == Guid.Empty)
                throw new DomainException("A Category is required.");

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            ReorderLevel = reorderLevel;
            CategoryId = categoryId;

        }
    }
}
