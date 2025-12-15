using Demo_C_.Models.SOLID;

namespace Demo_C_.Services;

/// <summary>
/// Service for managing SOLID Demo state and operations
/// Demonstrates separation of concerns - UI logic in Razor, business logic in Service
/// </summary>
public class SOLIDDemoService
{
    // SRP Demo State
    private BadNotificationService _badService = new();
    private DataSaver _saver = new();
    private EmailSender _emailer = new();
    private string _srpResult = "";
    private List<string> _srpLog = new();

    // OCP Demo State
    private NotificationSender _notifSender = new();
    private string _selectedMessageType = "Email";
    private string _ocpResult = "";

    // LSP Demo State
    private List<string> _lspResults = new();
    private bool _lspCrashed;
    private string _lspError = "";

    // ISP Demo State
    private string _selectedPrinter = "Xerox";
    private string _ispResult = "";
    private bool _ispCrashed;
    private string _ispError = "";

    // DIP Demo State
    private SmartHomeSwitch _smartSwitch = new();
    private string _selectedDevice = "Light";
    private string _dipResult = "";

    // SRP Properties
    public string SrpResult => _srpResult;
    public List<string> SrpLog => _srpLog;

    // OCP Properties
    public string SelectedMessageType
    {
        get => _selectedMessageType;
        set => _selectedMessageType = value;
    }
    public string OcpResult => _ocpResult;
    public List<string> NotifSenderLog => _notifSender.Log.ToList();

    // LSP Properties
    public List<string> LspResults => _lspResults;
    public bool LspCrashed => _lspCrashed;
    public string LspError => _lspError;

    // ISP Properties
    public string SelectedPrinter
    {
        get => _selectedPrinter;
        set => _selectedPrinter = value;
    }
    public string IspResult => _ispResult;
    public bool IspCrashed => _ispCrashed;
    public string IspError => _ispError;

    // DIP Properties
    public string SelectedDevice
    {
        get => _selectedDevice;
        set => _selectedDevice = value;
    }
    public string DipResult => _dipResult;
    public SmartHomeSwitch SmartSwitch => _smartSwitch;

    // SRP Methods
    public void BadSaveAndEmail()
    {
        _badService = new();
        _srpResult = _badService.SaveAndEmail("User Profile", "user@example.com");
        _srpLog = _badService.Log.ToList();
    }

    public void GoodSave()
    {
        _saver = new();
        _srpResult = _saver.Save("User Profile");
        _srpLog = _saver.Log.ToList();
    }

    public void GoodEmail()
    {
        _emailer = new();
        _srpResult = _emailer.Send("user@example.com", "Profile Updated");
        _srpLog = _emailer.Log.ToList();
    }

    // OCP Methods
    public void SendNotification()
    {
        _notifSender = new();
        IMessage message = _selectedMessageType switch
        {
            "Email" => new EmailMessage { To = "user@example.com", Subject = "Hello!" },
            "SMS" => new SmsMessage { PhoneNumber = "+1234567890", Text = "Hello!" },
            "Push" => new PushMessage { DeviceId = "device-123", Title = "New Alert" },
            "Slack" => new SlackMessage { Channel = "general", Text = "Hello team!" },
            _ => new EmailMessage { To = "user@example.com", Subject = "Hello!" }
        };
        _ocpResult = _notifSender.SendNotification(message);
    }

    // LSP Methods
    public void LaunchBadBirds()
    {
        _lspResults.Clear();
        _lspCrashed = false;
        _lspError = "";

        List<Bird> birds = new() { new Duck(), new Eagle(), new Penguin() };
        
        foreach (var bird in birds)
        {
            try
            {
                _lspResults.Add(bird.Fly());
            }
            catch (NotSupportedException ex)
            {
                _lspCrashed = true;
                _lspError = ex.Message;
                _lspResults.Add($"💥 {bird.Name}: CRASHED!");
                break;
            }
        }
    }

    public void LaunchGoodBirds()
    {
        _lspResults.Clear();
        _lspCrashed = false;
        _lspError = "";

        List<IFlyable> flyables = new() { new FlyingDuck(), new FlyingEagle() };
        
        foreach (var flyable in flyables)
        {
            _lspResults.Add(flyable.Fly());
        }
        _lspResults.Add("✅ All flights successful! Penguin couldn't be added to this list.");
    }

    // ISP Methods
    private IMultiFunctionDevice GetSelectedDevice() => 
        _selectedPrinter == "Xerox" ? new ModernXerox() : new OldHPPrinter();

    public void DoPrint()
    {
        _ispCrashed = false;
        _ispError = "";
        try
        {
            _ispResult = GetSelectedDevice().Print("Important Document.pdf");
        }
        catch (NotSupportedException ex)
        {
            _ispCrashed = true;
            _ispError = ex.Message;
        }
    }

    public void DoScan()
    {
        _ispCrashed = false;
        _ispError = "";
        try
        {
            _ispResult = GetSelectedDevice().Scan();
        }
        catch (NotSupportedException ex)
        {
            _ispCrashed = true;
            _ispError = ex.Message;
        }
    }

    public void DoFax()
    {
        _ispCrashed = false;
        _ispError = "";
        try
        {
            _ispResult = GetSelectedDevice().Fax("+1-555-0123");
        }
        catch (NotSupportedException ex)
        {
            _ispCrashed = true;
            _ispError = ex.Message;
        }
    }

    // DIP Methods
    public void ConnectDevice()
    {
        _smartSwitch = new();
        ISwitchable device = _selectedDevice switch
        {
            "Light" => new LightBulb(),
            "Fan" => new CeilingFan(),
            "Coffee" => new CoffeeMachine(),
            _ => new LightBulb()
        };
        _smartSwitch.ConnectDevice(device);
        _dipResult = $"Connected to {device.Name}";
    }

    public void ToggleDevice()
    {
        _dipResult = _smartSwitch.Toggle();
    }
}
