using UnityEngine;
using TMPro;

public class PlayerClassSelectorController : MonoBehaviour
{
	public TextMeshProUGUI label;
	public PlayerClass playerClass;

	void Start()
	{
	}

	void Update()
	{
	}

	public void SetClass(string name, PlayerClass playerClass)
	{
		this.playerClass = playerClass;

		label.text = name;
	}

	public void Click()
	{
		// spawner.player.playerClass = playerClass;

		// spawner.playerClassSelector.SetActive(false);
		// spawner.difficultySelector.SetActive(true);
	}
}
