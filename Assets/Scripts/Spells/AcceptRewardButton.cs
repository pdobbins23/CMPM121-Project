using UnityEngine;

public class AcceptRewardButton : MonoBehaviour
{
	public GameObject rewardPanel;
	public PlayerController playerObj;
	public GameObject[] dropButtons = new GameObject[4];

	public void Click()
	{
		var player = playerObj.spellcaster;
		var rewardSpell = rewardPanel.GetComponent<SpellRewardUI>().rewardSpell;
		
        if (player.spells.Count < 4)
        {
            player.spells.Add(rewardSpell);
            playerObj.spellui[player.spells.Count - 1].SetActive(true);
            playerObj.spellui[player.spells.Count - 1].GetComponent<SpellUI>().SetSpell(rewardSpell);

			foreach (var btn in dropButtons)
			{
				btn.SetActive(false);
			}

			rewardPanel.SetActive(false);
        }
	}
}
