using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class PlayerClassManager
{
	public Dictionary<string, PlayerClass> playerClasses = new Dictionary<string, PlayerClass>();

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

		JToken jo = JToken.Parse(jsonFile.text);
		
        foreach (var playerClass in jo.Children<JProperty>())
        {
			JObject obj = (JObject)playerClass.Value;
			
            playerClasses[playerClass.Name] = obj.ToObject<PlayerClass>();
        }
	}
}
