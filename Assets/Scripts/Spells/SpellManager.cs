using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SpellManager
{
    public Dictionary<string, RawSpell> AllSpells = new Dictionary<string, RawSpell>();

    private static SpellManager theInstance;
    public static SpellManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new SpellManager();
            return theInstance;
        }
    }

    private SpellManager()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("spells");
        JToken jo = JToken.Parse(jsonFile.text);

        foreach (var spell in jo.Children<JProperty>())
        {
            JObject obj = (JObject)spell.Value;
            
            AllSpells[spell.Name] = obj.ToObject<RawSpell>();
        }
    }
}
