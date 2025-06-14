#nullable enable

using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public struct AttrDamage
{
    public string Amount { get; set; }
    public string Type { get; set; }
}

public struct AttrProjectile
{
    public string Trajectory { get; set; }
    public string Speed { get; set; }
    public int Sprite { get; set; }
    public string? Lifetime { get; set; }
}

public class BaseSpell
{
    public string Name { get; set; } = "Unknown";
    public string Description { get; set; } = "Missing description.";

    public string ManaCost { get; set; } = "0";
    public AttrDamage? Damage { get; set; }
    public string? SecondaryDamage { get; set; }
    public string? Cooldown { get; set; }
    public int Icon { get; set; }
    public string? Count { get; set; }
    public string? Spray { get; set; }
    public AttrProjectile? Projectile { get; set; }
    public AttrProjectile? SecondaryProjectile { get; set; }

    public BaseSpell() { }

    public BaseSpell(BaseSpell other)
    {
        Name = other.Name;
        Description = other.Description;
        ManaCost = other.ManaCost;
        Damage = other.Damage;
        SecondaryDamage = other.SecondaryDamage;
        Cooldown = other.Cooldown;
        Icon = other.Icon;
        Count = other.Count;
        Spray = other.Spray;
        Projectile = other.Projectile;
        SecondaryProjectile = other.SecondaryProjectile;
    }
}

public class ModifierSpell
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public bool Modifier { get; set; }
    public bool? DoubleProjectile { get; set; }
    public bool? SplitProjectile { get; set; }
    public string? ProjectileTrajectory { get; set; }

    public string? DamageAdder { get; set; }
    public string? SpeedAdder { get; set; }
    public string? ManaAdder { get; set; }
    public string? CooldownAdder { get; set; }

    public string? DamageMultiplier { get; set; }
    public string? SpeedMultiplier { get; set; }
    public string? ManaMultiplier { get; set; }
    public string? CooldownMultiplier { get; set; }

    public string? Delay { get; set; }
    public string? Angle { get; set; }

    public void Modify(ModifierSpell other)
    {
        if (!other.Modifier) return;
        Modifier = true;

        DoubleProjectile = other.DoubleProjectile ?? DoubleProjectile;
        SplitProjectile = other.SplitProjectile ?? SplitProjectile;
        ProjectileTrajectory = other.ProjectileTrajectory ?? ProjectileTrajectory;

        DamageAdder = other.DamageAdder ?? DamageAdder;
        SpeedAdder = other.SpeedAdder ?? SpeedAdder;
        ManaAdder = other.ManaAdder ?? ManaAdder;
        CooldownAdder = other.CooldownAdder ?? CooldownAdder;

        DamageMultiplier = other.DamageMultiplier ?? DamageMultiplier;
        SpeedMultiplier = other.SpeedMultiplier ?? SpeedMultiplier;
        ManaMultiplier = other.ManaMultiplier ?? ManaMultiplier;
        CooldownMultiplier = other.CooldownMultiplier ?? CooldownMultiplier;

        Delay = other.Delay ?? Delay;
        Angle = other.Angle ?? Angle;

        if (other.Name != "")
            Name = Name == "" ? other.Name : $"{other.Name} {Name}";

        if (other.Description != "")
            Description = Description == "" ? other.Description : $"{Description}\n{other.Description}";
    }
}

public class RawSpell : BaseSpell
{
    public ModifierSpell Mod;

    public RawSpell(BaseSpell baseSpell, ModifierSpell? mod = null) : base(baseSpell) {
        Mod = mod ?? new ModifierSpell();
    }

    public string GetName()
    {
        return Mod.Name == "" ? Name : $"{Mod.Name} {Name}";
    }

    public string GetDescription()
    {
        return Mod.Description == "" ? Description : $"{Description}\n{Mod.Description}";
    }

    public static RawSpell? CraftSpell(RawSpell modifierSpell, RawSpell baseSpell)
    {
        if (!modifierSpell.Mod.Modifier)
        {
            if (!baseSpell.Mod.Modifier) return null;
            (modifierSpell, baseSpell) = (baseSpell, modifierSpell);
        }

        RawSpell craftedSpell = (RawSpell)baseSpell.MemberwiseClone();
        craftedSpell.Mod = modifierSpell.Mod;
        return craftedSpell;
    }
}
