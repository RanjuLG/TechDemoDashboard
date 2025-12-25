using TechDemoDashboard.Models;

namespace TechDemoDashboard.Services;

/// <summary>
/// CQRS Service demonstrating Command Query Responsibility Segregation
/// Separates write operations (Commands) from read operations (Queries)
/// Includes event sourcing for complete audit trail
/// </summary>
public class CQRSService
{
    // Write Model Store (Commands modify this)
    private readonly Dictionary<Guid, Product> _writeStore = new();

    // Read Model Store (Queries read from this - could be a different database)
    private readonly Dictionary<Guid, ProductReadModel> _readStore = new();

    // Event Store (Event Sourcing)
    private readonly List<DomainEvent> _eventStore = new();

    // Activity Log for UI
    public List<string> ActivityLog { get; } = new();

    // Current query results
    public List<ProductReadModel> QueryResults { get; private set; } = new();
    public string LastQueryDescription { get; private set; } = string.Empty;

    public CQRSService()
    {
        // Seed with sample data
        SeedData();
    }

    private void SeedData()
    {
        var sampleProducts = new[]
        {
            new { Name = "Laptop", Price = 999.99m, Stock = 15 },
            new { Name = "Mouse", Price = 29.99m, Stock = 50 },
            new { Name = "Keyboard", Price = 79.99m, Stock = 8 },
            new { Name = "Monitor", Price = 299.99m, Stock = 12 }
        };

        foreach (var p in sampleProducts)
        {
            HandleCreateProduct(new CreateProductCommand(p.Name, p.Price, p.Stock));
        }

        ActivityLog.Clear(); // Clear seed logs
        ActivityLog.Add("✅ System initialized with sample products");
    }

    // ============================================================================
    // COMMAND HANDLERS (Write Operations)
    // ============================================================================

    public Guid HandleCreateProduct(CreateProductCommand command)
    {
        // 1. Create domain entity
        var product = new Product(command.Name, command.Price, command.Stock);

        // 2. Store in write model
        _writeStore[product.Id] = product;

        // 3. Publish event
        var @event = new ProductCreatedEvent(product.Id, command.Name, command.Price, command.Stock);
        PublishEvent(@event);

        // 4. Update read model (projection)
        ProjectToReadModel(product);

        // 5. Log activity
        ActivityLog.Add($"📝 COMMAND: CreateProduct - '{command.Name}' (${command.Price}, Stock: {command.Stock})");

        return product.Id;
    }

    public void HandleUpdatePrice(UpdateProductPriceCommand command)
    {
        if (!_writeStore.TryGetValue(command.ProductId, out var product))
        {
            ActivityLog.Add($"❌ COMMAND FAILED: Product not found (ID: {command.ProductId})");
            return;
        }

        var oldPrice = product.Price;
        product.UpdatePrice(command.NewPrice);

        var @event = new ProductPriceChangedEvent(product.Id, oldPrice, command.NewPrice);
        PublishEvent(@event);

        ProjectToReadModel(product);

        ActivityLog.Add($"📝 COMMAND: UpdatePrice - '{product.Name}' from ${oldPrice:F2} to ${command.NewPrice:F2}");
    }

    public void HandleUpdateStock(UpdateProductStockCommand command)
    {
        if (!_writeStore.TryGetValue(command.ProductId, out var product))
        {
            ActivityLog.Add($"❌ COMMAND FAILED: Product not found (ID: {command.ProductId})");
            return;
        }

        var oldStock = product.Stock;
        product.UpdateStock(command.NewStock);

        var @event = new ProductStockChangedEvent(product.Id, oldStock, command.NewStock);
        PublishEvent(@event);

        ProjectToReadModel(product);

        ActivityLog.Add($"📝 COMMAND: UpdateStock - '{product.Name}' from {oldStock} to {command.NewStock} units");
    }

    public void HandleDeleteProduct(DeleteProductCommand command)
    {
        if (!_writeStore.TryGetValue(command.ProductId, out var product))
        {
            ActivityLog.Add($"❌ COMMAND FAILED: Product not found (ID: {command.ProductId})");
            return;
        }

        var productName = product.Name;
        product.Delete();

        var @event = new ProductDeletedEvent(product.Id, productName);
        PublishEvent(@event);

        // Remove from read model
        _readStore.Remove(product.Id);

        ActivityLog.Add($"📝 COMMAND: DeleteProduct - '{productName}' removed");
    }

    // ============================================================================
    // QUERY HANDLERS (Read Operations)
    // ============================================================================

    public List<ProductReadModel> HandleGetAllProducts(GetAllProductsQuery query)
    {
        QueryResults = _readStore.Values.OrderBy(p => p.Name).ToList();
        LastQueryDescription = $"Get All Products - Found {QueryResults.Count} products";
        ActivityLog.Add($"🔍 QUERY: {LastQueryDescription}");
        return QueryResults;
    }

    public ProductReadModel? HandleGetProductById(GetProductByIdQuery query)
    {
        _readStore.TryGetValue(query.ProductId, out var product);
        QueryResults = product != null ? new List<ProductReadModel> { product } : new();
        LastQueryDescription = product != null
            ? $"Get Product By ID - Found '{product.Name}'"
            : $"Get Product By ID - Not found";
        ActivityLog.Add($"🔍 QUERY: {LastQueryDescription}");
        return product;
    }

    public List<ProductReadModel> HandleGetLowStockProducts(GetLowStockProductsQuery query)
    {
        QueryResults = _readStore.Values
            .Where(p => p.Stock < query.Threshold)
            .OrderBy(p => p.Stock)
            .ToList();
        LastQueryDescription = $"Get Low Stock Products (< {query.Threshold}) - Found {QueryResults.Count} products";
        ActivityLog.Add($"🔍 QUERY: {LastQueryDescription}");
        return QueryResults;
    }

    public List<ProductReadModel> HandleGetProductsByPriceRange(GetProductsByPriceRangeQuery query)
    {
        QueryResults = _readStore.Values
            .Where(p => p.Price >= query.MinPrice && p.Price <= query.MaxPrice)
            .OrderBy(p => p.Price)
            .ToList();
        LastQueryDescription = $"Get Products By Price Range (${query.MinPrice:F2} - ${query.MaxPrice:F2}) - Found {QueryResults.Count} products";
        ActivityLog.Add($"🔍 QUERY: {LastQueryDescription}");
        return QueryResults;
    }

    // ============================================================================
    // EVENT SOURCING & PROJECTIONS
    // ============================================================================

    private void PublishEvent(DomainEvent @event)
    {
        _eventStore.Add(@event);
        ActivityLog.Add($"📨 EVENT: {@event.EventType} published (Event #{_eventStore.Count})");
    }

    private void ProjectToReadModel(Product product)
    {
        var eventCount = _eventStore.Count(e => e.AggregateId == product.Id);
        _readStore[product.Id] = new ProductReadModel(product, eventCount);
    }

    // ============================================================================
    // HELPER METHODS FOR UI
    // ============================================================================

    public List<DomainEvent> GetEventStore() => _eventStore.ToList();

    public List<Product> GetAllProductsFromWriteStore() => _writeStore.Values.Where(p => !p.IsDeleted).ToList();

    public void ClearActivityLog() => ActivityLog.Clear();

    public string GetArchitectureSummary()
    {
        return $"Write Store: {_writeStore.Count(p => !p.Value.IsDeleted)} products | " +
               $"Read Store: {_readStore.Count} models | " +
               $"Event Store: {_eventStore.Count} events";
    }
}
