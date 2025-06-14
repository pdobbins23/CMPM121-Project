using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public Hittable.Team team;

    public float spell_power = 10f;
    public List<Spell> spells = new List<Spell>();

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            AddMana(mana_reg);

            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
    }

    public void AddMana(int added_mana)
    {
        mana = Mathf.Min(mana + added_mana, max_mana);
    }

    public IEnumerator Cast(int index, Vector3 where, Vector3 target)
    {
        var spell = spells[index];

        int manaCost = spell.GetManaCost();

        if (mana >= manaCost && spell.IsReady())
        {
            mana -= manaCost;
            EventBus.Instance.DoCast();
            yield return spell.Cast(where, target, team);
        }

        yield break;
    }

    public float Evaluate(string expression)
    {
        return new Evaluator(new() { { "power", this.spell_power }, { "wave", GameManager.Instance.currentWave } }).Evaluate(expression);
    }

    public int EvaluateInt(string expression)
    {
        return new Evaluator(new() { { "power", this.spell_power }, { "wave", GameManager.Instance.currentWave } }).EvaluateInt(expression);
    }
}
