using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Domain.Inventory
{
    public sealed class Category : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private Category() { }

        public static Category Create (string name, string? description)
        {
            var category = new Category();
            category.Apply(name, description);
            return category;
        }

        private void Apply(string name, string? description)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");

            if (name.Trim().Length > 100)
                throw new DomainException("Category name cannot exceed 100 characters");

            Name = name.Trim();
            Description = description?.Trim();
        }
    }
}
