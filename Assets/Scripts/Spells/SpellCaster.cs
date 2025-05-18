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
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            
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

    public IEnumerator Cast(int index, Vector3 where, Vector3 target)
    {
        var spell = spells[index];
        
        int manaCost = spell.GetManaCost();
        
        if (mana >= manaCost && spell.IsReady())
        {
            mana -= manaCost;
            yield return spell.Cast(where, target, team);
        }
        
        yield break;
    }

    public SpellContext GetContext()
    {
        return new SpellContext
        {
            Power = this.spell_power,
            Wave = GameManager.Instance.currentWave // or however you track waves
        };
    }
}
