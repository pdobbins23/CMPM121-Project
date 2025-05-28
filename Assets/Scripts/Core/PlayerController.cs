using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public PlayerClass playerClass;
    
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public GameObject[] spellui = new GameObject[4];
    public GameObject[] dropButtons = new GameObject[4];

    public int speed;
    public int currentSpell = 0;

    public Unit unit;

    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;
    }

    public void StartLevel()
    {
        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        
        Spell startingSpell = new Spell(SpellManager.Instance.AllSpells["arcane_bolt"], spellcaster);
        
        spellcaster.spells.Add(startingSpell);
        
        StartCoroutine(spellcaster.ManaRegeneration());
        
        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellui[0].GetComponent<SpellUI>().SetSpell(spellcaster.spells[0]);

        // test
        // Spell spell2 = new Spell(SpellManager.Instance.AllSpells["arcane_blast"], spellcaster);
        // Spell spell3 = new Spell(SpellManager.Instance.AllSpells["magic_missile"], spellcaster);
        // Spell spell4 = new Spell(SpellManager.Instance.AllSpells["arcane_spray"], spellcaster);
        // spellcaster.spells.Add(spell2);
        // spellcaster.spells.Add(spell3);
        // spellcaster.spells.Add(spell4);
        // spellui[1].GetComponent<SpellUI>().SetSpell(spellcaster.spells[1]);
        // spellui[2].GetComponent<SpellUI>().SetSpell(spellcaster.spells[2]);
        // spellui[3].GetComponent<SpellUI>().SetSpell(spellcaster.spells[3]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSpell(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSpell(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSpell(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSpell(3);
        if (Input.GetKeyDown(KeyCode.K)) GameManager.Instance.KillAllEnemies();
        RelicManager.Instance.Update();
    }

    public void SelectSpell(int index) {
        if (index >= spellcaster.spells.Count) return;
        currentSpell = index;
        UpdateSpellUi();
    }

    public void DropSpell(int index)
    {
        if (index >= spellcaster.spells.Count) return;
        if (currentSpell > index || (currentSpell == index && index > 0)) currentSpell--;
        spellcaster.spells.RemoveAt(index);
        UpdateSpellUi();
    }

    public void UpdateSpellUi()
    {
        for (int i = 0; i < 4; i++)
            spellui[i].SetActive(false);

        foreach (var btn in dropButtons)
            btn.SetActive(false);

        for (int i = 0; i < spellcaster.spells.Count; i++)
        {
            spellui[i].SetActive(true);
            spellui[i].GetComponent<SpellUI>().SetSpell(spellcaster.spells[i]);
            spellui[i].GetComponent<SpellUI>().SetHighlight(i == currentSpell ? Color.red : Color.black);
        }
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(currentSpell, transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>()*speed;
    }

    void Die()
    {
        Debug.Log("You Lost");
        
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }

}
