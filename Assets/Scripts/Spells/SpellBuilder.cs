using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;


public class SpellBuilder
{
    private System.Random random = new System.Random();
    
    private List<RawSpell> baseSpells = new List<RawSpell>();
    private List<RawSpell> modifierSpells = new List<RawSpell>();

    public SpellBuilder()
    {
        foreach (var spell in SpellManager.Instance.AllSpells.Values)
        {
            if (spell.Modifier == true)
                modifierSpells.Add(spell);
            else
                baseSpells.Add(spell);
        }
    }

    public Spell GetRandomSpell(SpellCaster owner)
    {
        RawSpell spell = baseSpells[random.Next(baseSpells.Count)];

        int numModifiers = random.Next(0, 3);
        for (int i = 0; i < numModifiers; i++)
        {
            RawSpell modifierSpell = modifierSpells[random.Next(modifierSpells.Count)];

            spell = spell.WithModifierSpell(modifierSpell);
        }
        
        return new Spell(spell, owner);
    }
}
