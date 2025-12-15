using Demo_C_.Models;

namespace Demo_C_.Services;

/// <summary>
/// Service for managing OOP Demo state and operations
/// Demonstrates separation of concerns - UI logic in Razor, business logic in Service
/// </summary>
public class OOPDemoService
{
    private Hero? _selectedHero;
    private string _lastAction = "";
    private string _lastSound = "";
    private int _lastDamage;
    private string _encapsulationResult = "";

    public Hero? SelectedHero => _selectedHero;
    public string LastAction => _lastAction;
    public string LastSound => _lastSound;
    public int LastDamage => _lastDamage;
    public string EncapsulationResult => _encapsulationResult;

    public void SelectHero(Hero hero)
    {
        _selectedHero = hero;
        _lastAction = $"{hero.Name} the {hero.Class} enters the arena!";
        _lastSound = "🎺";
        _lastDamage = 0;
    }

    public void SelectWarrior() => SelectHero(new Warrior("Ragnar"));
    public void SelectMage() => SelectHero(new Mage("Ravana"));
    public void SelectRogue() => SelectHero(new Rogue("Shadow"));

    public void ChangeHero()
    {
        _selectedHero = null;
        _lastAction = "";
        _lastSound = "";
        _encapsulationResult = "";
    }

    public void PerformAttack()
    {
        if (_selectedHero == null || !_selectedHero.IsAlive) return;
        
        var (message, sound, damage) = _selectedHero.Attack();
        _lastAction = message;
        _lastSound = sound;
        _lastDamage = damage;
    }

    public void DealDamage(int amount)
    {
        if (_selectedHero == null) return;
        _selectedHero.TakeDamage(amount);
        _lastAction = $"{_selectedHero.Name} takes {amount} damage!";
        _lastSound = "💥";
        _lastDamage = 0;
    }

    public void HealHero(int amount)
    {
        if (_selectedHero == null) return;
        _selectedHero.Heal(amount);
        _lastAction = $"{_selectedHero.Name} heals!";
        _lastSound = "💚";
        _lastDamage = 0;
    }

    public void TryInvalidHealth()
    {
        if (_selectedHero == null) return;
        _encapsulationResult = _selectedHero.AttemptInvalidHealth(-100);
    }

    public void ResetHero()
    {
        if (_selectedHero == null) return;
        _selectedHero.ResetHealth();
        _lastAction = $"{_selectedHero.Name} is ready for battle!";
        _lastSound = "🔄";
        _lastDamage = 0;
        _encapsulationResult = "";
    }
}
