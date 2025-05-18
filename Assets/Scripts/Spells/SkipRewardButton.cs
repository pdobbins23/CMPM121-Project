using UnityEngine;

public class SkipRewardButton : MonoBehaviour
{
	public GameObject rewardPanel;
	public GameObject[] dropButtons = new GameObject[4];

	public void Click()
	{
		foreach (var btn in dropButtons)
		{
			btn.SetActive(false);
		}

		rewardPanel.SetActive(false);
	}
}
