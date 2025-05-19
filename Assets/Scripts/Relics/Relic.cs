using UnityEngine;

// Root data class parsed from relics.json
[System.Serializable]
public class RelicData
{
    public string name;
    public int sprite;
    public RelicTriggerData trigger;
    public RelicEffectData effect;
}

// Trigger information parsed from JSON
[System.Serializable]
public class RelicTriggerData
{
    public string description;
    public string type;
    public string amount;  // Optional, used in "stand-still" and custom triggers
}

// Effect information parsed from JSON
[System.Serializable]
public class RelicEffectData
{
    public string description;
    public string type;
    public string amount;
    public string until;  // Optional, used for temporary effects (e.g. "until": "cast-spell")
}

// Main runtime object that holds the parsed relic and connects the trigger and effect
public class Relic
{
    public string Name { get; private set; }
    public Sprite Icon { get; private set; }

    public IRelicTrigger Trigger { get; private set; }
    public IRelicEffect Effect { get; private set; }

    public Relic(RelicData data)
    {
        Name = data.name;
        Icon = SpriteManager.GetSpriteById(data.sprite); // Implement SpriteManager yourself

        Effect = EffectFactory.Create(data.effect);
        Trigger = TriggerFactory.Create(data.trigger);

        if (Trigger != null)
        {
            Trigger.Register(this);
        }
    }
}

// Trigger interface
public interface IRelicTrigger
{
    void Register(Relic relic);  // Attach trigger to an event
}

// Effect interface
public interface IRelicEffect
{
    void ApplyEffect();
    void RemoveEffectIfNeeded();
}

// Placeholder factories (to be filled with logic later)
public static class TriggerFactory
{
    public static IRelicTrigger Create(RelicTriggerData data)
    {
        switch (data.type)
        {
            case "take-damage": return new TakeDamageTrigger();
            case "stand-still": return new StandStillTrigger(float.Parse(data.amount));
            case "on-kill": return new OnKillTrigger();
            default:
                Debug.LogWarning("Unknown trigger type: " + data.type);
                return null;
        }
    }
}

public static class EffectFactory
{
    public static IRelicEffect Create(RelicEffectData data)
    {
        switch (data.type)
        {
            case "gain-mana": return new GainManaEffect(data.amount);
            case "gain-spellpower": return new GainSpellPowerEffect(data.amount, data.until);
            default:
                Debug.LogWarning("Unknown effect type: " + data.type);
                return null;
        }
    }
}
