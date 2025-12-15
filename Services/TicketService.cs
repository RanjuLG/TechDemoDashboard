namespace Demo_C_.Services;

/// <summary>
/// Service demonstrating race conditions and locking in concurrent scenarios.
/// Shows the classic "double-booking" / "overselling" problem.
/// </summary>
public class TicketService
{
    private int _availableTickets;
    private readonly object _lock = new();
    private readonly List<string> _log = new();

    public int AvailableTickets => _availableTickets;
    public IReadOnlyList<string> Log => _log.AsReadOnly();

    /// <summary>
    /// Resets the ticket count and clears the log.
    /// </summary>
    public void Reset(int ticketCount = 1)
    {
        _availableTickets = ticketCount;
        _log.Clear();
        _log.Add($"🎫 Initialized with {ticketCount} ticket(s)");
    }

    /// <summary>
    /// Attempts to buy a ticket WITHOUT locking.
    /// This demonstrates the race condition where multiple threads
    /// can read the same value before any of them decrement it.
    /// </summary>
    public (bool success, string message) TryBuyTicketUnsafe(int buyerId)
    {
        // Read current value
        var currentTickets = _availableTickets;
        var threadId = Environment.CurrentManagedThreadId;
        
        _log.Add($"👤 Buyer {buyerId} (Thread {threadId}): Checking tickets... sees {currentTickets} available");
        
        // Check if tickets available
        if (currentTickets > 0)
        {
            // Simulate database latency / processing time
            // This delay is what allows the race condition to occur
            Thread.Sleep(10);
            
            // Decrement (BUG: another thread may have already decremented!)
            _availableTickets--;
            
            _log.Add($"✅ Buyer {buyerId} (Thread {threadId}): PURCHASED! Tickets now: {_availableTickets}");
            return (true, $"Buyer {buyerId} purchased a ticket");
        }
        else
        {
            _log.Add($"❌ Buyer {buyerId} (Thread {threadId}): No tickets available");
            return (false, $"Buyer {buyerId} could not purchase - sold out");
        }
    }

    /// <summary>
    /// Attempts to buy a ticket WITH proper locking.
    /// This prevents the race condition by ensuring only one thread
    /// can read, check, and modify the ticket count at a time.
    /// </summary>
    public (bool success, string message) TryBuyTicketSafe(int buyerId)
    {
        lock (_lock)
        {
            var currentTickets = _availableTickets;
            var threadId = Environment.CurrentManagedThreadId;
            
            _log.Add($"🔒 Buyer {buyerId} (Thread {threadId}): Acquired lock, sees {currentTickets} available");
            
            if (currentTickets > 0)
            {
                // Simulate database latency / processing time
                Thread.Sleep(10);
                
                _availableTickets--;
                
                _log.Add($"✅ Buyer {buyerId} (Thread {threadId}): PURCHASED! Tickets now: {_availableTickets}");
                return (true, $"Buyer {buyerId} purchased a ticket");
            }
            else
            {
                _log.Add($"❌ Buyer {buyerId} (Thread {threadId}): No tickets available (lock released)");
                return (false, $"Buyer {buyerId} could not purchase - sold out");
            }
        }
    }

    /// <summary>
    /// Simulates multiple concurrent buyers attempting to purchase tickets.
    /// </summary>
    public (int successCount, int failCount, int finalTickets) SimulateConcurrentPurchases(int buyerCount, bool useLocking)
    {
        Reset(1); // Start with 1 ticket
        
        int successCount = 0;
        int failCount = 0;

        _log.Add($"");
        _log.Add($"🚀 Starting {buyerCount} concurrent purchase attempts...");
        _log.Add($"🔧 Locking: {(useLocking ? "ENABLED ✅" : "DISABLED ❌")}");
        _log.Add($"");

        Parallel.For(0, buyerCount, new ParallelOptions { MaxDegreeOfParallelism = buyerCount }, i =>
        {
            var (success, _) = useLocking 
                ? TryBuyTicketSafe(i + 1) 
                : TryBuyTicketUnsafe(i + 1);
            
            if (success)
                Interlocked.Increment(ref successCount);
            else
                Interlocked.Increment(ref failCount);
        });

        _log.Add($"");
        _log.Add($"📊 Results:");
        _log.Add($"   Successful purchases: {successCount}");
        _log.Add($"   Failed purchases: {failCount}");
        _log.Add($"   Final ticket count: {_availableTickets}");

        if (!useLocking && _availableTickets < 0)
        {
            _log.Add($"");
            _log.Add($"⚠️ OVERSOLD by {Math.Abs(_availableTickets)} tickets! This is the race condition bug.");
        }
        else if (useLocking && _availableTickets == 0 && successCount == 1)
        {
            _log.Add($"");
            _log.Add($"✨ Perfect! Lock prevented overselling. Exactly 1 ticket sold.");
        }

        return (successCount, failCount, _availableTickets);
    }
}
