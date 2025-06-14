using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SpellManager
{
    public Dictionary<string, RawSpell> AllSpells = new Dictionary<string, RawSpell>();

    public List<BaseSpell> BaseSpells = new();
    public List<ModifierSpell> ModifierSpells = new();

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

        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
            {
                NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
            }
        };

        JsonSerializer serializer = JsonSerializer.Create(settings);

        foreach (var spell in jo.Children<JProperty>())
        {
            JObject obj = (JObject)spell.Value;

            if ((bool?)obj["modifier"] == true)
            {
                var modifierSpell = obj.ToObject<ModifierSpell>(serializer);
                ModifierSpells.Add(modifierSpell);
            }
            else
            {
                var baseSpell = obj.ToObject<BaseSpell>(serializer);
                BaseSpells.Add(baseSpell);

                AllSpells[spell.Name] = new RawSpell(baseSpell);
            }
        }
    }
}
