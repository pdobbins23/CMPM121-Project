using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class EnemyManager{
    public Dictionary<string, Enemy> enemyTypes = new Dictionary<string, Enemy>();

    private static EnemyManager theInstance;
    public static EnemyManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new EnemyManager();
            return theInstance;
        }
    }

    private EnemyManager(){
        TextAsset jsonFile = Resources.Load<TextAsset>("enemies");

        JToken jo = JToken.Parse(jsonFile.text);

        foreach (var enemy in jo)
        {
            Enemy en = enemy.ToObject<Enemy>();
            enemyTypes[en.name] = en;
        }
    }
}

