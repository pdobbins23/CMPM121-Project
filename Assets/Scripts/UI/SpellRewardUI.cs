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

    public Image[] relicIcons = new Image[3];
    public TextMeshProUGUI[] relicDescs = new TextMeshProUGUI[3];
    public GameObject[] takeRelicButtons = new GameObject[3];

    public GameObject[] dropButtons = new GameObject[4];
    public Button declineButton;
    public Button acceptButton;

    public Spell rewardSpell;
    public RelicData[] relics = new RelicData[3];
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

        var rnd = new System.Random();
        var ar = RelicManager.Instance.AllRelics;

        for (int i = 0; i < 3; i++) {
            relics[i] = ar[rnd.Next(ar.Count)];

            relicIcons[i].gameObject.SetActive(true);
            relicDescs[i].gameObject.SetActive(true);
            takeRelicButtons[i].SetActive(true);

            // TODO: Set the image of relicIcons[i]
            relicDescs[i].text = relics[i].trigger.description + ", " + relics[i].effect.description;
        }

        panel.SetActive(true);
    }
}
