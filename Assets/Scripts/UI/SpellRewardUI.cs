using System.Collections.Generic;
using System.Linq;
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
        /*
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
        var r = RelicManager.Instance.ActiveRelics;

        var availableRelics = ar
            .Where(data => !r.Any(active => active.Name == data.name))
            .ToList();

        var chosenRelics = new List<RelicData>();

        for (int i = 0; i < 3; i++) {
            var eligible = availableRelics
                .Where(data => !chosenRelics.Any(chosen => chosen.name == data.name))
                .ToList();

            if (eligible.Count() == 0 || GameManager.Instance.currentWave % 3 != 0) {
                relicIcons[i].SetActive(false);
                relicDescs[i].SetActive(false);
                takeRelicButtons[i].SetActive(false);
                continue;
            }

            var chosen = eligible[rnd.Next(eligible.Count())];
            chosenRelics.Add(chosen);
            relics[i] = chosen;

            relicIcons[i].SetActive(true);
            relicDescs[i].SetActive(true);
            takeRelicButtons[i].SetActive(true);

            GameManager.Instance.relicIconManager.PlaceSprite(
                chosen.sprite,
                relicIcons[i].GetComponent<Image>()
            );

            relicDescs[i].GetComponent<TextMeshProUGUI>().text =
                chosen.trigger.description + ", " + chosen.effect.description;
        }

        panel.SetActive(true);
        */
    }
}
