#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public struct RawSpell
{
    public struct Damage
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public struct Projectile
    {
        [JsonProperty("trajectory")]
        public string Trajectory { get; set; }
        [JsonProperty("speed")]
        public string Speed { get; set; }
        [JsonProperty("sprite")]
        public int Sprite { get; set; }
        [JsonProperty("lifetime")]
        public string LifeTime { get; set; }
    }

    // Base stuff
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("mana_cost")]
    public string ManaCost { get; set; }
    [JsonProperty("damage")]
    public Damage? BaseDamage { get; set; }
    [JsonProperty("secondary_damage")]
    public string? SecondaryDamage { get; set; }
    [JsonProperty("cooldown")]
    public string? CoolDown { get; set; }
    [JsonProperty("icon")]
    public int Icon { get; set; }
    [JsonProperty("N")]
    public string? Count { get; set; }
    [JsonProperty("spray")]
    public string? Spray { get; set; }
    [JsonProperty("projectile")]
    public Projectile? BaseProjectile { get; set; }
    [JsonProperty("secondary_projectile")]
    public Projectile? SecondaryProjectile { get; set; }
    
    // Modifier stuff
    [JsonProperty("modifier")]
    public bool? Modifier { get; set; }
    [JsonProperty("double_projectile")]
    public bool? DoubleProjectile { get; set; }
    [JsonProperty("split_projectile")]
    public bool? SplitProjectile { get; set; }
    [JsonProperty("projectile_trajectory")]
    public string? ProjectileTrajectory { get; set; }

    [JsonProperty("damage_adder")]
    public string? DamageAdder { get; set; }
    [JsonProperty("speed_adder")]
    public string? SpeedAdder { get; set; }
    [JsonProperty("mana_adder")]
    public string? ManaAdder { get; set; }
    [JsonProperty("cooldown_adder")]
    public string? CoolDownAdder { get; set; }

    [JsonProperty("damage_multiplier")]
    public string? DamageMultiplier { get; set; }
    [JsonProperty("speed_multiplier")]
    public string? SpeedMultiplier { get; set; }
    [JsonProperty("mana_multiplier")]
    public string? ManaMultiplier { get; set; }
    [JsonProperty("cooldown_multiplier")]
    public string? CoolDownMultiplier { get; set; }

    [JsonProperty("delay")]
    public string? Delay { get; set; }
    [JsonProperty("angle")]
    public string? Angle { get; set; }

    /// Combine two spells.
    /// Used to apply modifier spells to base spells.
    public RawSpell WithModifierSpell(RawSpell modifierSpell)
    {
        var rs = new RawSpell();

        // TODO: Fix this whole thing somehow, combine spells better.

        // Base spell elements
        rs.Name = $"{Name} ({modifierSpell.Name})";
        // rs.Description = Description;
        // rs.ManaCost = ManaCost;
        // rs.BaseDamage = BaseDamage;
        // rs.SecondaryDamage = SecondaryDamage;
        // rs.CoolDown = CoolDown;
        // rs.Icon = Icon;
        // rs.Count = Count;
        // rs.BaseProjectile = BaseProjectile;
        // rs.SecondaryProjectile = SecondaryProjectile;

        // Modifier stuff
        // rs.DoubleProjectile = modifierSpell.DoubleProjectile;
        // rs.SplitProjectile = modifierSpell.SplitProjectile;
        // rs.ProjectileTrajectory = modifierSpell.ProjectileTrajectory;
        // rs.DamageAdder = modifierSpell.DamageAdder;
        // rs.SpeedAdder = modifierSpell.SpeedAdder;
        // rs.ManaAdder = modifierSpell.ManaAdder;
        // rs.CoolDownAdder = modifierSpell.CoolDownAdder;
        // rs.DamageMultiplier = modifierSpell.DamageMultiplier;
        // rs.SpeedMultiplier = modifierSpell.SpeedMultiplier;
        // rs.ManaMultiplier = modifierSpell.ManaMultiplier;
        // rs.CoolDownMultiplier = modifierSpell.CoolDownMultiplier;
        // rs.Delay = modifierSpell.Delay;
        // rs.Angle = modifierSpell.Angle;

        return rs;
    }
}
