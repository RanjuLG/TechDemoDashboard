namespace TechDemoDashboard.Models;

/// <summary>
/// ABSTRACTION: Abstract base class that defines a blueprint for all heroes.
/// You cannot instantiate this directly - must use concrete implementations.
/// </summary>
public abstract class Hero
{
    // ENCAPSULATION: Private backing field - external code cannot access directly
    private int _health;
    private int _maxHealth;

    public string Name { get; protected set; }
    public string Class { get; protected set; }
    public string Icon { get; protected set; }
    public int MaxHealth => _maxHealth;
    public List<string> BattleLog { get; } = new();

    // ENCAPSULATION: Property with validation logic
    // Health cannot go below 0 or above MaxHealth
    public int Health
    {
        get => _health;
        protected set
        {
            if (value < 0)
                _health = 0;
            else if (value > _maxHealth)
                _health = _maxHealth;
            else
                _health = value;
        }
    }

    public bool IsAlive => Health > 0;
    public double HealthPercentage => (double)Health / MaxHealth * 100;

    protected Hero(string name, int maxHealth)
    {
        Name = name;
        _maxHealth = maxHealth;
        _health = maxHealth;
        Class = "Unknown";
        Icon = "👤";
    }

    // ABSTRACTION: Abstract method - must be implemented by subclasses
    // This defines WHAT should happen, not HOW
    public abstract (string message, string sound, int damage) Attack();

    // POLYMORPHISM: Virtual method - can be overridden with different behavior
    public virtual void TakeDamage(int damage)
    {
        var previousHealth = Health;
        Health -= damage;
        BattleLog.Add($"💥 {Name} took {damage} damage! ({previousHealth} → {Health} HP)");
    }

    // ENCAPSULATION: Controlled healing - prevents over-healing
    public void Heal(int amount)
    {
        var previousHealth = Health;
        Health += amount;
        BattleLog.Add($"💚 {Name} healed for {amount}! ({previousHealth} → {Health} HP)");
    }

    public void ResetHealth()
    {
        Health = MaxHealth;
        BattleLog.Clear();
        BattleLog.Add($"🔄 {Name} reset to full health!");
    }

    // ENCAPSULATION: Attempt to set invalid health - demonstrates validation
    public string AttemptInvalidHealth(int value)
    {
        var before = Health;
        Health = value;
        return $"Attempted to set health to {value}, but encapsulation enforced valid value: {before} → {Health}";
    }
}

/// <summary>
/// INHERITANCE: Warrior inherits from Hero and specializes with melee attacks.
/// </summary>
public class Warrior : Hero
{
    public int Strength { get; private set; }

    public Warrior(string name) : base(name, 120)  // Warriors have more HP
    {
        Class = "Warrior";
        Icon = "⚔️";
        Strength = 25;
    }

    // POLYMORPHISM: Warrior's attack is different from other heroes
    public override (string message, string sound, int damage) Attack()
    {
        var baseDamage = Strength + Random.Shared.Next(5, 15);
        var isCritical = Random.Shared.Next(100) < 20;  // 20% crit chance
        
        if (isCritical)
        {
            var critDamage = baseDamage * 2;
            BattleLog.Add($"⚔️ {Name} performs a CRITICAL sword strike! ({critDamage} damage)");
            return ($"{Name} lands a CRITICAL HIT!", "🗡️ *SLASH!*", critDamage);
        }
        
        BattleLog.Add($"⚔️ {Name} swings their sword! ({baseDamage} damage)");
        return ($"{Name} attacks with their sword!", "🗡️ *Clang!*", baseDamage);
    }

    // POLYMORPHISM: Warriors have additional damage reduction
    public override void TakeDamage(int damage)
    {
        var reducedDamage = (int)(damage * 0.85);  // 15% damage reduction (armor)
        BattleLog.Add($"🛡️ {Name}'s armor reduces damage by {damage - reducedDamage}!");
        base.TakeDamage(reducedDamage);
    }
}

/// <summary>
/// INHERITANCE: Mage inherits from Hero and specializes with magic attacks.
/// </summary>
public class Mage : Hero
{
    public int Intelligence { get; private set; }
    public int Mana { get; private set; }
    public int MaxMana { get; private set; }

    public Mage(string name) : base(name, 80)  // Mages have less HP
    {
        Class = "Mage";
        Icon = "🔮";
        Intelligence = 35;
        MaxMana = 100;
        Mana = 100;
    }

    // POLYMORPHISM: Mage's attack is completely different from Warrior
    public override (string message, string sound, int damage) Attack()
    {
        var spellCost = 15;
        
        if (Mana >= spellCost)
        {
            Mana -= spellCost;
            var baseDamage = Intelligence + Random.Shared.Next(10, 25);
            var isFireball = Random.Shared.Next(100) < 30;  // 30% chance for fireball
            
            if (isFireball)
            {
                var fireballDamage = (int)(baseDamage * 1.5);
                BattleLog.Add($"🔥 {Name} casts FIREBALL! ({fireballDamage} damage, -{spellCost} mana)");
                return ($"{Name} hurls a massive FIREBALL!", "🔥 *WHOOOOSH!*", fireballDamage);
            }
            
            BattleLog.Add($"✨ {Name} casts Arcane Bolt! ({baseDamage} damage, -{spellCost} mana)");
            return ($"{Name} casts Arcane Bolt!", "✨ *Zap!*", baseDamage);
        }
        
        // No mana - weak staff attack
        var staffDamage = 5;
        BattleLog.Add($"🪄 {Name} attacks with staff (no mana)! ({staffDamage} damage)");
        return ($"{Name} is out of mana! Weak staff attack...", "🪄 *bonk*", staffDamage);
    }

    public void RestoreMana(int amount)
    {
        var previousMana = Mana;
        Mana = Math.Min(Mana + amount, MaxMana);
        BattleLog.Add($"💙 {Name} restored {Mana - previousMana} mana! ({previousMana} → {Mana})");
    }
}

/// <summary>
/// INHERITANCE: Rogue inherits from Hero with stealth-based attacks.
/// </summary>
public class Rogue : Hero
{
    public int Agility { get; private set; }
    private bool _isStealthed;

    public Rogue(string name) : base(name, 90)
    {
        Class = "Rogue";
        Icon = "🗡️";
        Agility = 30;
        _isStealthed = true;
    }

    public override (string message, string sound, int damage) Attack()
    {
        var baseDamage = Agility + Random.Shared.Next(8, 18);
        
        if (_isStealthed)
        {
            _isStealthed = false;
            var backstabDamage = baseDamage * 3;  // Triple damage from stealth
            BattleLog.Add($"🔪 {Name} BACKSTABS from stealth! ({backstabDamage} damage)");
            return ($"{Name} emerges from shadows with a BACKSTAB!", "🔪 *CRITICAL!*", backstabDamage);
        }
        
        // Chance to re-stealth
        if (Random.Shared.Next(100) < 25)
        {
            _isStealthed = true;
            BattleLog.Add($"👤 {Name} vanishes into the shadows...");
        }
        
        BattleLog.Add($"🗡️ {Name} strikes with daggers! ({baseDamage} damage)");
        return ($"{Name} attacks with twin daggers!", "🗡️ *Slash slash!*", baseDamage);
    }

    // POLYMORPHISM: Rogues have evasion chance
    public override void TakeDamage(int damage)
    {
        if (Random.Shared.Next(100) < 25)  // 25% dodge chance
        {
            BattleLog.Add($"💨 {Name} dodges the attack completely!");
            return;
        }
        base.TakeDamage(damage);
    }
}
