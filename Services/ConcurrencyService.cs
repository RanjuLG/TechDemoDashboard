using System.Diagnostics;

namespace Demo_C_.Services;

/// <summary>
/// Service containing all concurrency and parallelism demo logic.
/// Separated from Razor files for easy debugging with breakpoints.
/// </summary>
public class ConcurrencyService
{
    private const int ItemCount = 5;
    private const int WorkDelayMs = 500;

    /// <summary>
    /// Demonstrates blocking sequential execution.
    /// The UI will freeze during execution.
    /// Place breakpoints here to debug synchronous flow.
    /// </summary>
    /// <returns>Execution time in milliseconds</returns>
    public long RunSequentialDemo(Action<string> onProgress)
    {
        var stopwatch = Stopwatch.StartNew();
        
        onProgress("🚀 Starting Sequential (Blocking) Demo...");
        onProgress($"Processing {ItemCount} items synchronously on Thread {Environment.CurrentManagedThreadId}");
        onProgress("");

        for (int i = 1; i <= ItemCount; i++)
        {
            // Simulate CPU-bound work (blocking)
            Thread.Sleep(WorkDelayMs);
            onProgress($"✅ Processed item {i}/{ItemCount} on Thread {Environment.CurrentManagedThreadId}");
        }

        stopwatch.Stop();
        onProgress("");
        onProgress("🏁 Sequential Demo Complete!");
        onProgress($"⏱️ Execution Time: {stopwatch.ElapsedMilliseconds}ms");
        onProgress("⚠️ Notice: UI was FROZEN during execution.");
        
        return stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Demonstrates async/await concurrency (non-blocking).
    /// The UI remains responsive during execution.
    /// Place breakpoints here to debug async flow.
    /// </summary>
    /// <returns>Execution time in milliseconds</returns>
    public async Task<long> RunAsyncDemo(Func<string, Task> onProgress)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await onProgress("🚀 Starting Async (Concurrency) Demo...");
        await onProgress($"Processing {ItemCount} items asynchronously on Thread {Environment.CurrentManagedThreadId}");
        await onProgress("");

        for (int i = 1; i <= ItemCount; i++)
        {
            // Simulate I/O-bound work (non-blocking)
            await Task.Delay(WorkDelayMs);
            await onProgress($"✅ Processed item {i}/{ItemCount} on Thread {Environment.CurrentManagedThreadId}");
        }

        stopwatch.Stop();
        await onProgress("");
        await onProgress("🏁 Async Demo Complete!");
        await onProgress($"⏱️ Execution Time: {stopwatch.ElapsedMilliseconds}ms");
        await onProgress("✨ Notice: UI remained RESPONSIVE during execution.");
        await onProgress("💡 Note: Items processed SEQUENTIALLY (one after another).");
        
        return stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Demonstrates true async concurrency using Task.WhenAll.
    /// All tasks run concurrently, completing much faster than sequential async.
    /// The UI remains responsive during execution.
    /// Place breakpoints here to debug concurrent async flow.
    /// </summary>
    /// <returns>Execution time in milliseconds</returns>
    public async Task<long> RunConcurrentDemo(Func<string, Task> onProgress)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await onProgress("🚀 Starting Concurrent (Task.WhenAll) Demo...");
        await onProgress($"Processing {ItemCount} items CONCURRENTLY on Thread {Environment.CurrentManagedThreadId}");
        await onProgress("");

        // Create all tasks upfront
        var tasks = Enumerable.Range(1, ItemCount).Select(async i =>
        {
            // Simulate I/O-bound work (all running concurrently)
            await Task.Delay(WorkDelayMs);
            await onProgress($"✅ Processed item {i}/{ItemCount} on Thread {Environment.CurrentManagedThreadId}");
        }).ToArray();

        // Wait for all tasks to complete concurrently
        await Task.WhenAll(tasks);

        stopwatch.Stop();
        await onProgress("");
        await onProgress("🏁 Concurrent Demo Complete!");
        await onProgress($"⏱️ Execution Time: {stopwatch.ElapsedMilliseconds}ms");
        await onProgress("✨ Notice: UI remained RESPONSIVE during execution.");
        await onProgress("🔥 All items processed CONCURRENTLY (much faster than sequential async!)");
        
        return stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Demonstrates CPU parallelism using Parallel.ForEach.
    /// Multiple threads process items simultaneously.
    /// Place breakpoints here to debug parallel execution.
    /// </summary>
    /// <param name="onProgress">Callback for progress updates</param>
    /// <param name="maxDegreeOfParallelism">Maximum number of concurrent threads</param>
    /// <returns>Execution time in milliseconds</returns>
    public long RunParallelDemo(Action<string> onProgress, int maxDegreeOfParallelism = 4)
    {
        var stopwatch = Stopwatch.StartNew();
        
        onProgress("🚀 Starting Parallel (CPU) Demo...");
        onProgress($"Processing {ItemCount} items in parallel using up to {maxDegreeOfParallelism} CPU threads");
        onProgress("");

        var items = Enumerable.Range(1, ItemCount).ToArray();
        var processedItems = new System.Collections.Concurrent.ConcurrentBag<string>();

        Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, item =>
        {
            var threadId = Environment.CurrentManagedThreadId;
            
            // Simulate CPU-bound work
            Thread.Sleep(WorkDelayMs);
            
            var message = $"✅ Processed item {item}/{ItemCount} on Thread {threadId}";
            processedItems.Add(message);
            onProgress(message);
        });

        stopwatch.Stop();
        onProgress("");
        onProgress("🏁 Parallel Demo Complete!");
        onProgress($"⏱️ Execution Time: {stopwatch.ElapsedMilliseconds}ms");
        onProgress($"🧵 Used multiple threads for true parallelism.");
        onProgress("💡 Tip: Notice different Thread IDs in the log above.");
        
        return stopwatch.ElapsedMilliseconds;
    }
}
