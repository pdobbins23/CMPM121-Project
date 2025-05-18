using UnityEngine;

public class JsonUtils {
	public static T[] ReadJsonArray<T>(string json) {
		string fixedJson = "{ \"array\": " + json + "}";
		JsonArray<T> arr = JsonUtility.FromJson<JsonArray<T>>(fixedJson);
		return arr.array;
	}

	[System.Serializable]
	private class JsonArray<T> {
		public T[] array;
	}
}
