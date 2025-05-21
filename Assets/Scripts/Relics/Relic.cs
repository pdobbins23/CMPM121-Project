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
    public Sprite Icon { get; private set; }

    private RelicData data;
    private bool effectActive = false;
    private float timer = 0f;

    public Relic(RelicData relicData)
    {
        data = relicData;
        Name = relicData.name;
        Icon = SpriteManager.GetSpriteById(data.sprite);
        RegisterTrigger();
    }

    public void Update()
    {
        if (data.trigger.type == "stand-still")
        {
            if (!Player.Instance.IsMoving())
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

            case "move-distance":
                Player.Instance.OnMove += (distance) =>
                {
                    if (distance >= float.Parse(data.trigger.amount))
                        ApplyEffect();
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
                    Player.Instance.AddMana(mana);
                break;

            case "gain-spellpower":
                int amount = EvaluateAmount(data.effect.amount);
                Player.Instance.AddSpellPowerBuff(amount); // You handle this
                effectActive = true;
                break;

            case "gain-max-health":
            if (int.TryParse(data.effect.amount, out int hp))
                Player.Instance.IncreaseMaxHP(hp); // You implement this
                break;

        }
    }

    private void RemoveEffectIfNeeded()
    {
        if (effectActive && data.effect.type == "gain-spellpower")
        {
            int amount = EvaluateAmount(data.effect.amount);
            Player.Instance.RemoveSpellPowerBuff(amount);
            effectActive = false;
        }
    }

    private int EvaluateAmount(string expr)
    {
        var pc = GameManager.Instance.player.GetComponent<PlayerController>();
        var ctx = pc.spellcaster.GetContext().ToDictionary();

        return (int) RPN.eval(expr, ctx);
    }
}
