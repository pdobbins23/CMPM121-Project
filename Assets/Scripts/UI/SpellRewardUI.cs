using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellRewardUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI spellNameText;
    public TextMeshProUGUI spellDescText;
    public Image iconImage;

    public GameObject[] relicIcons;
    public GameObject[] relicDescs;
    public GameObject[] takeRelicButtons;

    public GameObject[] dropButtons;
    public Button declineButton;
    public Button acceptButton;

    public Spell rewardSpell;
    public RelicData[] relics = new RelicData[3];
    private SpellCaster player;
    public PlayerController playerObj;

    void Awake()
    {
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

        var rnd = new System.Random();
        var ar = RelicManager.Instance.AllRelics;

        for (int i = 0; i < 3; i++) {
            relics[i] = ar[rnd.Next(ar.Count)];

            relicIcons[i].SetActive(true);
            relicDescs[i].SetActive(true);
            takeRelicButtons[i].SetActive(true);

            // TODO: Set the image of relicIcons[i]
            relicDescs[i].GetComponent<TextMeshProUGUI>().text = relics[i].trigger.description + ", " + relics[i].effect.description;
        }

        panel.SetActive(true);
    }
}
