using TechDemoDashboard.Models;

namespace TechDemoDashboard.Services;

/// <summary>
/// Service for managing Delegates & Events demo state
/// Handles stock exchange and trader subscriptions
/// </summary>
public class DelegatesEventsService
{
    private StockExchange _exchange;
    private BullTrader _bullTrader;
    private BearTrader _bearTrader;
    private DayTrader _dayTrader;
    private NewsAlert _newsAlert;

    public List<string> GlobalActivityLog { get; } = new();

    // Subscription states
    public bool IsBullSubscribed { get; private set; }
    public bool IsBearSubscribed { get; private set; }
    public bool IsDayTraderSubscribed { get; private set; }
    public bool IsNewsSubscribed { get; private set; }

    // Expose current state
    public StockExchange Exchange => _exchange;
    public BullTrader BullTrader => _bullTrader;
    public BearTrader BearTrader => _bearTrader;
    public DayTrader DayTrader => _dayTrader;
    public NewsAlert NewsAlert => _newsAlert;

    public DelegatesEventsService()
    {
        // Initialize the stock exchange
        _exchange = new StockExchange("TECH", 100.00m);
        
        // Initialize traders
        _bullTrader = new BullTrader("Warren Bull");
        _bearTrader = new BearTrader("George Bear");
        _dayTrader = new DayTrader("Quick Trader");
        _newsAlert = new NewsAlert("Market News");

        // Subscribe all by default
        SubscribeBull();
        SubscribeBear();
        SubscribeDayTrader();
        SubscribeNews();
    }

    public void ChangePrice()
    {
        GlobalActivityLog.Add($"💰 Price Change Event Fired!");
        _exchange.ChangePrice();
        GlobalActivityLog.Add($"   New Price: ${_exchange.CurrentPrice:F2}");
        GlobalActivityLog.Add($"   Active Subscribers responded:");
        
        // Add recent activities from all traders
        AddRecentActivities();
    }

    private void AddRecentActivities()
    {
        if (_bullTrader.ActivityLog.Any())
            GlobalActivityLog.Add($"   {_bullTrader.ActivityLog.Last()}");
        if (_bearTrader.ActivityLog.Any())
            GlobalActivityLog.Add($"   {_bearTrader.ActivityLog.Last()}");
        if (_dayTrader.ActivityLog.Any())
            GlobalActivityLog.Add($"   {_dayTrader.ActivityLog.Last()}");
        if (_newsAlert.ActivityLog.Any())
            GlobalActivityLog.Add($"   {_newsAlert.ActivityLog.Last()}");
    }

    // SUBSCRIBE methods using += operator
    public void SubscribeBull()
    {
        if (!IsBullSubscribed)
        {
            _exchange.OnPriceChanged += _bullTrader.OnPriceChanged;
            IsBullSubscribed = true;
            GlobalActivityLog.Add($"✅ Subscribed: {_bullTrader.Name}");
        }
    }

    public void SubscribeBear()
    {
        if (!IsBearSubscribed)
        {
            _exchange.OnPriceChanged += _bearTrader.OnPriceChanged;
            IsBearSubscribed = true;
            GlobalActivityLog.Add($"✅ Subscribed: {_bearTrader.Name}");
        }
    }

    public void SubscribeDayTrader()
    {
        if (!IsDayTraderSubscribed)
        {
            _exchange.OnPriceChanged += _dayTrader.OnPriceChanged;
            IsDayTraderSubscribed = true;
            GlobalActivityLog.Add($"✅ Subscribed: {_dayTrader.Name}");
        }
    }

    public void SubscribeNews()
    {
        if (!IsNewsSubscribed)
        {
            _exchange.OnPriceChanged += _newsAlert.OnPriceChanged;
            IsNewsSubscribed = true;
            GlobalActivityLog.Add($"✅ Subscribed: {_newsAlert.Source}");
        }
    }

    // UNSUBSCRIBE methods using -= operator
    public void UnsubscribeBull()
    {
        if (IsBullSubscribed)
        {
            _exchange.OnPriceChanged -= _bullTrader.OnPriceChanged;
            IsBullSubscribed = false;
            GlobalActivityLog.Add($"❌ Unsubscribed: {_bullTrader.Name}");
        }
    }

    public void UnsubscribeBear()
    {
        if (IsBearSubscribed)
        {
            _exchange.OnPriceChanged -= _bearTrader.OnPriceChanged;
            IsBearSubscribed = false;
            GlobalActivityLog.Add($"❌ Unsubscribed: {_bearTrader.Name}");
        }
    }

    public void UnsubscribeDayTrader()
    {
        if (IsDayTraderSubscribed)
        {
            _exchange.OnPriceChanged -= _dayTrader.OnPriceChanged;
            IsDayTraderSubscribed = false;
            GlobalActivityLog.Add($"❌ Unsubscribed: {_dayTrader.Name}");
        }
    }

    public void UnsubscribeNews()
    {
        if (IsNewsSubscribed)
        {
            _exchange.OnPriceChanged -= _newsAlert.OnPriceChanged;
            IsNewsSubscribed = false;
            GlobalActivityLog.Add($"❌ Unsubscribed: {_newsAlert.Source}");
        }
    }

    public void ToggleBull()
    {
        if (IsBullSubscribed) UnsubscribeBull();
        else SubscribeBull();
    }

    public void ToggleBear()
    {
        if (IsBearSubscribed) UnsubscribeBear();
        else SubscribeBear();
    }

    public void ToggleDayTrader()
    {
        if (IsDayTraderSubscribed) UnsubscribeDayTrader();
        else SubscribeDayTrader();
    }

    public void ToggleNews()
    {
        if (IsNewsSubscribed) UnsubscribeNews();
        else SubscribeNews();
    }

    public void ClearLogs()
    {
        GlobalActivityLog.Clear();
        _bullTrader.ActivityLog.Clear();
        _bearTrader.ActivityLog.Clear();
        _dayTrader.ActivityLog.Clear();
        _newsAlert.ActivityLog.Clear();
    }
}
