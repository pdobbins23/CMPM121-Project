using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class RelicManager
{
    public List<Relic> AllRelics = new List<Relic>();
    public List<Relic> ActiveRelics = new List<Relic>();

    private static RelicManager theInstance;
    public static RelicManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new RelicManager();
            return theInstance;
        }
    }

    private RelicManager()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("relics");
        if (jsonFile == null)
        {
            Debug.LogError("relics.json not found in Resources folder!");
            return;
        }

        JArray relicArray = JArray.Parse(jsonFile.text);
        foreach (var relic in relicArray.Children<JObject>())
        {
            AllRelics.Add(new Relic(relic.ToObject<RelicData>()));
        }
    }

    public void Update()
    {
        foreach (var relic in ActiveRelics)
        {
            relic.Update();
        }
    }
}
