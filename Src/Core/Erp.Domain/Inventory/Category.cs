using Erp.Domain.Common;
using Erp.Domain.Exceptions;

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

        public void Update(string name, string? description)
        {
            Apply(name, description);
            MarkUpdated();
        }

        private void Apply(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");

            var cleanName = name.Trim();
            if (cleanName.Length > 100)
                throw new DomainException("Category name cannot exceed 100 characters.");

            var cleanDescription = description?.Trim();
            if (cleanDescription is { Length: > 500 })
                throw new DomainException("Description cannot exceed 500 characters.");

            Name = cleanName;
            Description = cleanDescription;
        }
    }
}
