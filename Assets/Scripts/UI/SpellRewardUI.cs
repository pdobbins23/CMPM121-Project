using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellRewardUI : MonoBehaviour
{
    public static SpellRewardUI Instance;

    public GameObject panel;
    public TextMeshProUGUI spellNameText;
    public TextMeshProUGUI spellDescText;
    public Image iconImage;

    public GameObject[] dropButtons = new GameObject[4];
    public Button declineButton;
    public Button acceptButton;

    public Spell rewardSpell;
    private SpellCaster player;
    public PlayerController playerObj;

    public SpellRewardUI()
    {
        Instance = this;
    }

    void Awake()
    {
        Instance = this;
        // panel.SetActive(false);
    }

    public void Show(Spell newSpell, SpellCaster caster)
    {
        rewardSpell = newSpell;
        player = caster;

        spellNameText.text = rewardSpell.GetName();
        spellDescText.text = $"Mana: {rewardSpell.GetManaCost()}\n" +
                             $"Damage: {rewardSpell.GetBaseDamage()}\n" +
                             $"Cooldown: {rewardSpell.GetCoolDown()}";

        GameManager.Instance.spellIconManager.PlaceSprite(rewardSpell.GetIcon(), iconImage);

        if (player.spells.Count > 3)
        {
            foreach (var btn in dropButtons)
            {
                btn.SetActive(true);
            }
        }

        panel.SetActive(true);
    }
}
