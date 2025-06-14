using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;


public class SpellBuilder
{
    private System.Random random = new System.Random();

    public Spell GetRandomSpell(SpellCaster owner)
    {
        var baseSpells = SpellManager.Instance.BaseSpells;
        var modifierSpells = SpellManager.Instance.ModifierSpells;

        RawSpell spell = new RawSpell(baseSpells[random.Next(baseSpells.Count)]);

        int numModifiers = random.Next(0, 3);
        for (int i = 0; i < numModifiers; i++)
            spell.Mod.Modify(modifierSpells[random.Next(modifierSpells.Count)]);

        return new Spell(spell, owner);
    }
}
