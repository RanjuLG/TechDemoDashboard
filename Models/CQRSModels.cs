namespace TechDemoDashboard.Models;

// ============================================================================
// CQRS - Command Query Responsibility Segregation
// ============================================================================

// ============================================================================
// DOMAIN MODELS (Write Model)
// ============================================================================

/// <summary>
/// Write model - Used by commands to modify state
/// Contains business logic and validation
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public bool IsDeleted { get; set; }

    public Product(string name, decimal price, int stock)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
        LastModified = DateTime.UtcNow;
    }

    public void UpdateStock(int newStock)
    {
        Stock = newStock;
        LastModified = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        LastModified = DateTime.UtcNow;
    }
}

// ============================================================================
// READ MODELS (Optimized for queries)
// ============================================================================

/// <summary>
/// Read model - Optimized for display purposes
/// May contain denormalized/aggregated data
/// </summary>
public class ProductReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string StockStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public int TotalEvents { get; set; }

    public ProductReadModel(Product product, int totalEvents)
    {
        Id = product.Id;
        Name = product.Name;
        Price = product.Price;
        Stock = product.Stock;
        CreatedAt = product.CreatedAt;
        LastModified = product.LastModified;
        TotalEvents = totalEvents;

        // Computed field for display
        StockStatus = Stock switch
        {
            0 => "Out of Stock",
            < 10 => "Low Stock",
            < 50 => "In Stock",
            _ => "Well Stocked"
        };
    }
}

// ============================================================================
// COMMANDS (Write Operations)
// ============================================================================

public abstract class Command
{
    public Guid CommandId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class CreateProductCommand : Command
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public CreateProductCommand(string name, decimal price, int stock)
    {
        Name = name;
        Price = price;
        Stock = stock;
    }
}

public class UpdateProductPriceCommand : Command
{
    public Guid ProductId { get; set; }
    public decimal NewPrice { get; set; }

    public UpdateProductPriceCommand(Guid productId, decimal newPrice)
    {
        ProductId = productId;
        NewPrice = newPrice;
    }
}

public class UpdateProductStockCommand : Command
{
    public Guid ProductId { get; set; }
    public int NewStock { get; set; }

    public UpdateProductStockCommand(Guid productId, int newStock)
    {
        ProductId = productId;
        NewStock = newStock;
    }
}

public class DeleteProductCommand : Command
{
    public Guid ProductId { get; set; }

    public DeleteProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}

// ============================================================================
// QUERIES (Read Operations)
// ============================================================================

public abstract class Query
{
    public Guid QueryId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class GetAllProductsQuery : Query { }

public class GetProductByIdQuery : Query
{
    public Guid ProductId { get; set; }

    public GetProductByIdQuery(Guid productId)
    {
        ProductId = productId;
    }
}

public class GetLowStockProductsQuery : Query
{
    public int Threshold { get; set; } = 10;
}

public class GetProductsByPriceRangeQuery : Query
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    public GetProductsByPriceRangeQuery(decimal minPrice, decimal maxPrice)
    {
        MinPrice = minPrice;
        MaxPrice = maxPrice;
    }
}

// ============================================================================
// EVENTS (Event Sourcing)
// ============================================================================

public abstract class DomainEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public Guid AggregateId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; }
}

public class ProductCreatedEvent : DomainEvent
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public ProductCreatedEvent(Guid productId, string name, decimal price, int stock)
    {
        AggregateId = productId;
        EventType = "ProductCreated";
        Name = name;
        Price = price;
        Stock = stock;
    }
}

public class ProductPriceChangedEvent : DomainEvent
{
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }

    public ProductPriceChangedEvent(Guid productId, decimal oldPrice, decimal newPrice)
    {
        AggregateId = productId;
        EventType = "ProductPriceChanged";
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

public class ProductStockChangedEvent : DomainEvent
{
    public int OldStock { get; set; }
    public int NewStock { get; set; }

    public ProductStockChangedEvent(Guid productId, int oldStock, int newStock)
    {
        AggregateId = productId;
        EventType = "ProductStockChanged";
        OldStock = oldStock;
        NewStock = newStock;
    }
}

public class ProductDeletedEvent : DomainEvent
{
    public string ProductName { get; set; }

    public ProductDeletedEvent(Guid productId, string productName)
    {
        AggregateId = productId;
        EventType = "ProductDeleted";
        ProductName = productName;
    }
}
