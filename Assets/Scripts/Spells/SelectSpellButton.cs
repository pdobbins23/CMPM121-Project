using UnityEngine;

public class SelectSpellButton : MonoBehaviour
{
	public PlayerController player;

	public void Select(int spell)
	{
		player.currentSpell = spell;
		Debug.Log("what");
	}
}
