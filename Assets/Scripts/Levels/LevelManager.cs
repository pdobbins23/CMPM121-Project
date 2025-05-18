using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class LevelManager {
    public Dictionary<string, Level> levelTypes = new Dictionary<string, Level>();

    private static LevelManager theInstance;
    public static LevelManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new LevelManager();
            return theInstance;
        }
    }

    private LevelManager(){
        TextAsset jsonFile = Resources.Load<TextAsset>("levels");

        JToken jo = JToken.Parse(jsonFile.text);

        foreach (var level in jo)
        {
            Level lvl = level.ToObject<Level>();
            levelTypes[lvl.name] = lvl;
        }
    }
}
