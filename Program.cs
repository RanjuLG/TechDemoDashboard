using TechDemoDashboard.Components;
using TechDemoDashboard.Services;
using TechDemoDashboard.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register custom services
builder.Services.AddScoped<ConcurrencyService>();

// Register DI Lifetime demo services
builder.Services.AddTransient<TransientService>();
builder.Services.AddScoped<ScopedService>();
builder.Services.AddSingleton<SingletonService>();

// Register Race Condition demo service (Singleton to persist state)
builder.Services.AddSingleton<TicketService>();

// Register OOP and SOLID demo services (Scoped for per-request state)
builder.Services.AddScoped<OOPDemoService>();
builder.Services.AddScoped<SOLIDDemoService>();
builder.Services.AddScoped<DelegatesEventsService>();
builder.Services.AddScoped<CQRSService>();

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add controller support for potential API endpoints
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Map API controllers
app.MapControllers();

// Map SignalR hub
app.MapHub<ChatHub>("/chathub");

app.MapRazorComponents<TechDemoDashboard.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
;
