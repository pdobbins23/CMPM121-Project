using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class PlayerClassManager
{
	public List<PlayerClass> playerClasses = new List<PlayerClass>();

	private static PlayerClassManager theInstance;
	public static PlayerClassManager Instance
	{
		get
		{
			if (theInstance == null)
				theInstance = new PlayerClassManager();
			return theInstance;
		}
	}

	private PlayerClassManager()
	{
        TextAsset jsonFile = Resources.Load<TextAsset>("classes");
        if (jsonFile == null)
        {
            Debug.LogError("classes.json not found in Resources folder!");
            return;
        }

        JArray playerClassArray = JArray.Parse(jsonFile.text);
        foreach (var playerClass in playerClassArray.Children<JObject>())
        {
            playerClasses.Add(playerClass.ToObject<PlayerClass>());
        }
	}
}
