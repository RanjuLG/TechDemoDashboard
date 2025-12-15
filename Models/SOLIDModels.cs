namespace TechDemoDashboard.Models.SOLID;

// ============================================================================
// S - SINGLE RESPONSIBILITY PRINCIPLE (SRP)
// ============================================================================

/// <summary>
/// BAD: One class doing multiple things - saving AND emailing
/// </summary>
public class BadNotificationService
{
    public List<string> Log { get; } = new();

    public string SaveAndEmail(string data, string email)
    {
        // This method violates SRP - it does TWO things
        Log.Add($"💾 Saving data: {data}");
        Log.Add($"📧 Sending email to: {email}");
        Log.Add($"⚠️ PROBLEM: One method doing two jobs!");
        return $"Saved '{data}' and emailed to {email}";
    }
}

/// <summary>
/// GOOD: Separate class responsible ONLY for saving
/// </summary>
public class DataSaver
{
    public List<string> Log { get; } = new();

    public string Save(string data)
    {
        Log.Add($"💾 DataSaver.Save() - Only responsible for saving");
        Log.Add($"   Saved: {data}");
        return $"Data saved: {data}";
    }
}

/// <summary>
/// GOOD: Separate class responsible ONLY for emailing
/// </summary>
public class EmailSender
{
    public List<string> Log { get; } = new();

    public string Send(string email, string subject)
    {
        Log.Add($"📧 EmailSender.Send() - Only responsible for emails");
        Log.Add($"   Sent to: {email}");
        return $"Email sent to: {email}";
    }
}

// ============================================================================
// O - OPEN/CLOSED PRINCIPLE (OCP)
// ============================================================================

/// <summary>
/// Abstraction: Any message type can be sent
/// OPEN for extension (new message types), CLOSED for modification
/// </summary>
public interface IMessage
{
    string MessageType { get; }
    string Send();
    string Icon { get; }
}

public class EmailMessage : IMessage
{
    public string To { get; set; } = "";
    public string Subject { get; set; } = "";
    public string MessageType => "Email";
    public string Icon => "📧";
    
    public string Send() => $"Email sent to {To}: {Subject}";
}

public class SmsMessage : IMessage
{
    public string PhoneNumber { get; set; } = "";
    public string Text { get; set; } = "";
    public string MessageType => "SMS";
    public string Icon => "📱";
    
    public string Send() => $"SMS sent to {PhoneNumber}: {Text}";
}

public class PushMessage : IMessage
{
    public string DeviceId { get; set; } = "";
    public string Title { get; set; } = "";
    public string MessageType => "Push Notification";
    public string Icon => "🔔";
    
    public string Send() => $"Push sent to device {DeviceId}: {Title}";
}

public class SlackMessage : IMessage
{
    public string Channel { get; set; } = "";
    public string Text { get; set; } = "";
    public string MessageType => "Slack";
    public string Icon => "💬";
    
    public string Send() => $"Slack message to #{Channel}: {Text}";
}

/// <summary>
/// NotificationSender NEVER needs to change when new message types are added
/// It's CLOSED for modification, OPEN for extension via new IMessage types
/// </summary>
public class NotificationSender
{
    public List<string> Log { get; } = new();

    public string SendNotification(IMessage message)
    {
        Log.Add($"🚀 NotificationSender.SendNotification()");
        Log.Add($"   Received: {message.MessageType}");
        var result = message.Send();
        Log.Add($"   Result: {result}");
        Log.Add($"✅ This class NEVER changes for new message types!");
        return result;
    }
}

// ============================================================================
// L - LISKOV SUBSTITUTION PRINCIPLE (LSP)
// ============================================================================

/// <summary>
/// BAD: Base class that not all children can properly implement
/// </summary>
public abstract class Bird
{
    public string Name { get; protected set; } = "";
    public abstract string Fly();
}

public class Duck : Bird
{
    public Duck() { Name = "Duck"; }
    public override string Fly() => "🦆 Duck is flying gracefully!";
}

/// <summary>
/// VIOLATION: Penguin inherits Bird but can't actually fly!
/// This breaks LSP - you can't substitute Penguin where Bird is expected
/// </summary>
public class Penguin : Bird
{
    public Penguin() { Name = "Penguin"; }
    public override string Fly() => throw new NotSupportedException("🐧 CRASH! Penguins can't fly!");
}

public class Eagle : Bird
{
    public Eagle() { Name = "Eagle"; }
    public override string Fly() => "🦅 Eagle soars through the sky!";
}

/// <summary>
/// GOOD: Interface that only truly flyable things implement
/// </summary>
public interface IFlyable
{
    string Name { get; }
    string Fly();
}

public class FlyingDuck : IFlyable
{
    public string Name => "Duck";
    public string Fly() => "🦆 Duck is flying gracefully!";
}

public class FlyingEagle : IFlyable
{
    public string Name => "Eagle";
    public string Fly() => "🦅 Eagle soars through the sky!";
}

// Penguin simply doesn't implement IFlyable - it can't be in a flight list!

// ============================================================================
// I - INTERFACE SEGREGATION PRINCIPLE (ISP)
// ============================================================================

/// <summary>
/// BAD: Fat interface that forces classes to implement methods they don't need
/// </summary>
public interface IMultiFunctionDevice
{
    string Print(string document);
    string Scan();
    string Fax(string number);
}

/// <summary>
/// Modern device that can do everything - no problem here
/// </summary>
public class ModernXerox : IMultiFunctionDevice
{
    public string Print(string document) => $"🖨️ Xerox printing: {document}";
    public string Scan() => "📄 Xerox scanning document...";
    public string Fax(string number) => $"📠 Xerox faxing to {number}";
}

/// <summary>
/// VIOLATION: Old printer forced to implement methods it can't perform
/// </summary>
public class OldHPPrinter : IMultiFunctionDevice
{
    public string Print(string document) => $"🖨️ HP printing: {document}";
    public string Scan() => throw new NotSupportedException("💥 CRASH! This old printer can't scan!");
    public string Fax(string number) => throw new NotSupportedException("💥 CRASH! This old printer can't fax!");
}

/// <summary>
/// GOOD: Segregated interfaces - each device only implements what it can do
/// </summary>
public interface IPrinter { string Print(string document); }
public interface IScanner { string Scan(); }
public interface IFaxer { string Fax(string number); }

public class SmartXerox : IPrinter, IScanner, IFaxer
{
    public string Print(string document) => $"🖨️ Smart Xerox printing: {document}";
    public string Scan() => "📄 Smart Xerox scanning...";
    public string Fax(string number) => $"📠 Smart Xerox faxing to {number}";
}

public class SimplePrinter : IPrinter
{
    // Only implements what it can actually do!
    public string Print(string document) => $"🖨️ Simple printer: {document}";
    // No Scan() or Fax() methods to throw exceptions!
}

// ============================================================================
// D - DEPENDENCY INVERSION PRINCIPLE (DIP)
// ============================================================================

/// <summary>
/// Abstraction: Any switchable device
/// </summary>
public interface ISwitchable
{
    string Name { get; }
    string TurnOn();
    string TurnOff();
    bool IsOn { get; }
    string Icon { get; }
    string OnColor { get; }
}

public class LightBulb : ISwitchable
{
    public string Name => "Light Bulb";
    public string Icon => "💡";
    public string OnColor => "#ffd700";
    public bool IsOn { get; private set; }
    
    public string TurnOn() { IsOn = true; return "💡 Light bulb glows bright!"; }
    public string TurnOff() { IsOn = false; return "🌑 Light bulb is off."; }
}

public class CeilingFan : ISwitchable
{
    public string Name => "Ceiling Fan";
    public string Icon => "🌀";
    public string OnColor => "#4ecdc4";
    public bool IsOn { get; private set; }
    
    public string TurnOn() { IsOn = true; return "🌀 Fan starts spinning!"; }
    public string TurnOff() { IsOn = false; return "💨 Fan stops."; }
}

public class CoffeeMachine : ISwitchable
{
    public string Name => "Coffee Machine";
    public string Icon => "☕";
    public string OnColor => "#8b4513";
    public bool IsOn { get; private set; }
    
    public string TurnOn() { IsOn = true; return "☕ Coffee brewing!"; }
    public string TurnOff() { IsOn = false; return "⏸️ Coffee machine off."; }
}

/// <summary>
/// BAD: Hardcoded dependency - tightly coupled to LightBulb
/// </summary>
public class BadSmartSwitch
{
    private readonly LightBulb _bulb = new();  // HARDCODED!
    
    public string Toggle()
    {
        // Can only control a LightBulb, nothing else
        return _bulb.IsOn ? _bulb.TurnOff() : _bulb.TurnOn();
    }
}

/// <summary>
/// GOOD: Depends on abstraction, not concrete implementation
/// Can control ANY switchable device without changing code
/// </summary>
public class SmartHomeSwitch
{
    private ISwitchable? _device;
    public List<string> Log { get; } = new();

    public ISwitchable? CurrentDevice => _device;

    public void ConnectDevice(ISwitchable device)
    {
        _device = device;
        Log.Add($"🔌 Connected to: {device.Name}");
    }

    public string Toggle()
    {
        if (_device == null)
        {
            Log.Add("❌ No device connected!");
            return "No device connected";
        }

        Log.Add($"🔘 Toggle() called on SmartHomeSwitch");
        Log.Add($"   Current device: {_device.Name}");
        
        var result = _device.IsOn ? _device.TurnOff() : _device.TurnOn();
        
        Log.Add($"   Result: {result}");
        Log.Add($"✅ Same Toggle() code works for ANY device!");
        
        return result;
    }
}
