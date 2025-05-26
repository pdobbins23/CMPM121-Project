using UnityEngine;

public class TakeRelicButton : MonoBehaviour
{
	public GameObject rewardPanel;
	public PlayerController playerObj;
	public GameObject[] takeButtons = new GameObject[4];

	public void TakeRelic(int index)
	{
		var rewardUi = rewardPanel.GetComponent<SpellRewardUI>();
		var relic = rewardUi.relics[index];

		// make sure player doesn't already have this relic
		foreach (var r in playerObj.spellcaster.activeRelics) {
			if (r.Name == relic.Name) {
				return;
			}
		}

		playerObj.spellcaster.activeRelics.Add(relic);

		foreach (var btn in rewardUi.takeRelicButtons) {
			btn.SetActive(false);
		}

		foreach (var icon in rewardUi.relicIcons) {
			icon.SetActive(false);
		}

		foreach (var desc in rewardUi.relicDescs) {
			desc.SetActive(false);
		}
	}
}
