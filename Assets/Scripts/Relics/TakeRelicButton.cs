using UnityEngine;

public class TakeRelicButton : MonoBehaviour
{
	public GameObject rewardPanel;

	public void TakeRelic(int index)
	{
		var rewardUi = rewardPanel.GetComponent<SpellRewardUI>();
		var relic = rewardUi.relics[index];

		RelicManager.Instance.ActiveRelics.Add(new Relic(relic));

		foreach (var btn in rewardUi.takeRelicButtons) {
			btn.SetActive(false);
		}

		foreach (var icon in rewardUi.relicIcons) {
			icon.gameObject.SetActive(false);
		}

		foreach (var desc in rewardUi.relicDescs) {
			desc.gameObject.SetActive(false);
		}
	}
}
