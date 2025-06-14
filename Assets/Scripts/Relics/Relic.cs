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
    private RelicData data;
    private bool effectActive = false;
    private float timer = 0f;
    private float distancedMoved = 0;
    private PlayerController player;
    private bool _registered = false;

    private Action _onCastApply;
    private Action _onCastRemove;
    private Action<float> _onMoveRemove;
    private Action<float> _onMoveApply;
    private Action<Hittable> _onKill;
    private Action<Vector3, Damage, Hittable> _onDamage;
    private Action _onWaveStart;

    public Relic(RelicData relicData)
    {
        data = relicData;
        player = GameManager.Instance.player.GetComponent<PlayerController>();
    }

    public string GetName() => data.name;
    public string GetDescription() => data.trigger.description + data.effect.description;

    public int GetIcon() => data.sprite;

    public void Update()
    {
        RegisterTrigger();

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
        if (_registered) return;
        _registered = true;

        switch (data.trigger.type)
        {
            case "on-cast":
                _onCastApply = () => ApplyEffect();
                EventBus.Instance.OnCast += _onCastApply;
                break;

            case "take-damage":
                _onDamage = (where, damage, target) =>
                {
                    if (target.owner == GameManager.Instance.player) ApplyEffect();
                };
                EventBus.Instance.OnDamage += _onDamage;
                break;

            case "on-kill":
                _onKill = (target) =>
                {
                    if (target.owner != GameManager.Instance.player) ApplyEffect();
                };
                EventBus.Instance.OnKill += _onKill;
                break;

            case "move-distance":
                _onMoveApply = (distance) =>
                {
                    distancedMoved += distance;
                    if (distancedMoved >= float.Parse(data.trigger.amount))
                    {
                        distancedMoved = 0;
                        ApplyEffect();
                    }
                };
                EventBus.Instance.OnMove += _onMoveApply;
                break;

            case "wave-start":
                _onWaveStart = () => ApplyEffect();
                GameManager.Instance.OnWaveStart += _onWaveStart;
                break;
        }

        if (data.effect.until == "cast-spell")
        {
            _onCastRemove = () => RemoveEffectIfNeeded();
            EventBus.Instance.OnCast += _onCastRemove;
        }
        else if (data.effect.until == "move")
        {
            _onMoveRemove = (distance) => RemoveEffectIfNeeded();
            EventBus.Instance.OnMove += _onMoveRemove;
        }
    }

    public void UnregisterTrigger()
    {
        if (!_registered) return;
        _registered = false;

        if (_onCastApply != null) EventBus.Instance.OnCast -= _onCastApply;
        if (_onCastRemove != null) EventBus.Instance.OnCast -= _onCastRemove;
        if (_onDamage != null) EventBus.Instance.OnDamage -= _onDamage;
        if (_onKill != null) EventBus.Instance.OnKill -= _onKill;
        if (_onMoveApply != null) EventBus.Instance.OnMove -= _onMoveApply;
        if (_onMoveRemove != null) EventBus.Instance.OnMove -= _onMoveRemove;
        if (_onWaveStart != null) GameManager.Instance.OnWaveStart -= _onWaveStart;

        // Null them out if you want to be extra clean
        _onCastApply = null;
        _onCastRemove = null;
        _onDamage = null;
        _onKill = null;
        _onMoveApply = null;
        _onMoveRemove = null;
        _onWaveStart = null;
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
