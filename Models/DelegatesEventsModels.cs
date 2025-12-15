namespace TechDemoDashboard.Models;

// ============================================================================
// DELEGATES & EVENTS - Stock Market Simulation
// ============================================================================

/// <summary>
/// Custom EventArgs for price change notifications
/// </summary>
public class PriceChangedEventArgs : EventArgs
{
    public string Symbol { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal Change => NewPrice - OldPrice;
    public decimal ChangePercent => OldPrice != 0 ? (Change / OldPrice) * 100 : 0;

    public PriceChangedEventArgs(string symbol, decimal oldPrice, decimal newPrice)
    {
        Symbol = symbol;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

/// <summary>
/// PUBLISHER: Stock Exchange that fires price change events
/// Demonstrates how events protect the delegate (only the owner can fire it)
/// </summary>
public class StockExchange
{
    private decimal _currentPrice;
    private readonly Random _random = new();

    public string Symbol { get; }
    public decimal CurrentPrice => _currentPrice;

    // DELEGATE: Defines the signature for event handlers
    public delegate void PriceChangedHandler(object sender, PriceChangedEventArgs e);

    // EVENT: Wrapper around the delegate that protects it
    // Only StockExchange can fire this event (Invoke)
    // Others can only subscribe (+=) or unsubscribe (-=)
    public event PriceChangedHandler? OnPriceChanged;

    public StockExchange(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _currentPrice = initialPrice;
    }

    /// <summary>
    /// Changes the price and fires the event to notify all subscribers
    /// </summary>
    public void ChangePrice()
    {
        decimal oldPrice = _currentPrice;
        
        // Random price change between -10% and +10%
        decimal changePercent = (decimal)(_random.NextDouble() * 20 - 10);
        decimal newPrice = _currentPrice * (1 + changePercent / 100);
        newPrice = Math.Round(newPrice, 2);
        
        _currentPrice = newPrice;

        // Fire the event - this invokes all subscribed handlers
        OnPriceChanged?.Invoke(this, new PriceChangedEventArgs(Symbol, oldPrice, newPrice));
    }
}

/// <summary>
/// SUBSCRIBER: Bull Trader - Buys when price goes up
/// </summary>
public class BullTrader
{
    public string Name { get; }
    public List<string> ActivityLog { get; } = new();

    public BullTrader(string name)
    {
        Name = name;
    }

    /// <summary>
    /// This method matches the PriceChangedHandler delegate signature
    /// It can be subscribed to the OnPriceChanged event
    /// </summary>
    public void OnPriceChanged(object sender, PriceChangedEventArgs e)
    {
        if (e.Change > 0)
        {
            var message = $"🐂 {Name}: Price UP! Buying {e.Symbol} at ${e.NewPrice:F2} (+{e.ChangePercent:F1}%)";
            ActivityLog.Add(message);
        }
    }
}

/// <summary>
/// SUBSCRIBER: Bear Trader - Sells when price goes down
/// </summary>
public class BearTrader
{
    public string Name { get; }
    public List<string> ActivityLog { get; } = new();

    public BearTrader(string name)
    {
        Name = name;
    }

    public void OnPriceChanged(object sender, PriceChangedEventArgs e)
    {
        if (e.Change < 0)
        {
            var message = $"🐻 {Name}: Price DOWN! Selling {e.Symbol} at ${e.NewPrice:F2} ({e.ChangePercent:F1}%)";
            ActivityLog.Add(message);
        }
    }
}

/// <summary>
/// SUBSCRIBER: Day Trader - Trades on any significant change
/// </summary>
public class DayTrader
{
    public string Name { get; }
    public List<string> ActivityLog { get; } = new();

    public DayTrader(string name)
    {
        Name = name;
    }

    public void OnPriceChanged(object sender, PriceChangedEventArgs e)
    {
        if (Math.Abs(e.ChangePercent) > 5)
        {
            var action = e.Change > 0 ? "Trading HIGH volatility" : "Trading LOW volatility";
            var message = $"📈 {Name}: {action}! {e.Symbol} at ${e.NewPrice:F2} ({e.ChangePercent:+F1;-F1}%)";
            ActivityLog.Add(message);
        }
    }
}

/// <summary>
/// SUBSCRIBER: News Alert System - Reports all changes
/// </summary>
public class NewsAlert
{
    public string Source { get; }
    public List<string> ActivityLog { get; } = new();

    public NewsAlert(string source)
    {
        Source = source;
    }

    public void OnPriceChanged(object sender, PriceChangedEventArgs e)
    {
        var emoji = e.Change > 0 ? "📢" : "📰";
        var message = $"{emoji} {Source}: {e.Symbol} moved from ${e.OldPrice:F2} to ${e.NewPrice:F2}";
        ActivityLog.Add(message);
    }
}
