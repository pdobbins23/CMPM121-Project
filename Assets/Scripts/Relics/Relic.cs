using UnityEngine;
using System;

[System.Serializable]
public class RelicData
{
    public string name;
    public int sprite;
    public RelicTriggerData trigger;
    public RelicEffectData effect;
}

[System.Serializable]
public class RelicTriggerData
{
    public string description;
    public string type;
    public string amount;  // Optional
}

[System.Serializable]
public class RelicEffectData
{
    public string description;
    public string type;
    public string amount;
    public string until; // Optional
}

public class Relic
{
    public string Name { get; private set; }
    // public Sprite Icon { get; private set; }

    private RelicData data;
    private bool effectActive = false;
    private float timer = 0f;
    private float distancedMoved = 0;
    private PlayerController player;
    private bool _registered = false;

    public Relic(RelicData relicData)
    {
        data = relicData;
        Name = relicData.name;
        // Icon = RelicIconManager.GetSpriteById(data.sprite);
        player = GameManager.Instance.player.GetComponent<PlayerController>();
    }

    public int GetIcon() => data.sprite;

    public void Update()
    {
        if (!_registered)
        {
            RegisterTrigger();
            _registered = true;
        }

        if (data.trigger.type == "stand-still")
        {
            if (GameManager.Instance.player.GetComponent<Rigidbody2D>().linearVelocity.sqrMagnitude < 1)
            {
                timer += Time.deltaTime;
                if (!effectActive && timer >= float.Parse(data.trigger.amount))
                {
                    ApplyEffect();
                    effectActive = true;
                }
            }
            else
            {
                if (effectActive)
                    RemoveEffectIfNeeded();
                timer = 0f;
                effectActive = false;
            }
        }
    }

    private void RegisterTrigger()
    {
        switch (data.trigger.type)
        {
            case "on-cast":
                EventBus.Instance.OnCast += () => ApplyEffect();
                break;

            case "take-damage":
                EventBus.Instance.OnDamage += (where, damage, target) =>
                {
                    if (target.owner == GameManager.Instance.player) ApplyEffect();
                };
                break;

            case "on-kill":
                EventBus.Instance.OnKill += (target) =>
                {
                    if (target.owner != GameManager.Instance.player) ApplyEffect();
                };
                break;

            case "move-distance":
                EventBus.Instance.OnMove += (distance) =>
                {
                    distancedMoved += distance;
                    if (distancedMoved >= float.Parse(data.trigger.amount))
                    {
                        distancedMoved = 0;
                        ApplyEffect();
                    }
                };
                break;

            case "wave-start":
                GameManager.Instance.OnWaveStart += () => ApplyEffect();
                break;
        }

        if (data.effect.until == "cast-spell")
        {
            EventBus.Instance.OnCast += () => RemoveEffectIfNeeded();
        }
        else if (data.effect.until == "move")
        {
            EventBus.Instance.OnMove += (distance) => RemoveEffectIfNeeded();
        }
    }

    private void ApplyEffect()
    {
        switch (data.effect.type)
        {
            case "gain-mana":
                if (int.TryParse(data.effect.amount, out int mana))
                    player.spellcaster.AddMana(mana);
                break;

            case "gain-spellpower":
                int amount = EvaluateAmount(data.effect.amount);
                player.spellcaster.spell_power += amount;
                effectActive = true;
                break;

            case "gain-max-health":
                if (int.TryParse(data.effect.amount, out int hp))
                    player.hp.SetMaxHP(player.hp.max_hp + hp);
                break;

        }
    }

    private void RemoveEffectIfNeeded()
    {
        if (effectActive && data.effect.type == "gain-spellpower")
        {
            int amount = EvaluateAmount(data.effect.amount);
            player.spellcaster.spell_power -= amount;
            effectActive = false;
        }
    }

    private int EvaluateAmount(string expr)
    {
        return player.spellcaster.EvaluateInt(expr);
    }
}
