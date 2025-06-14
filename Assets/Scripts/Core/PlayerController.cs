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

    public int speed;

    public List<ItemSlot> Inventory = new();
    public List<ItemSlot> Equipments = new();

    public ItemSlot ActiveSlot = new();

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

        Inventory.Clear();
        for (int i = 0; i < 8; i++) Inventory.Add(new ItemSlot());
        Equipments.Clear();
        Equipments.Add(new ItemSlot(null, item => item.Equipment?.Type == 0));
        Equipments.Add(new ItemSlot(null, item => item.Equipment?.Type == 1));
        Equipments.Add(new ItemSlot(null, item => item.Equipment?.Type == 2));

        ActiveSlot = Inventory[0];
        ActiveSlot.Item = new Item(startingSpell);

        StartCoroutine(spellcaster.ManaRegeneration());

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(Inventory[0]);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(Inventory[1]);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(Inventory[2]);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(Inventory[3]);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(Inventory[4]);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(Inventory[5]);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(Inventory[6]);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(Inventory[7]);
        if (Input.GetKeyDown(KeyCode.K)) GameManager.Instance.KillAllEnemies();
        RelicManager.Instance.Update();
    }

    public void SelectSlot(ItemSlot slot) {
        ActiveSlot = slot;
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        if (Inventory.Contains(ActiveSlot) && ActiveSlot.Item?.Spell is Spell spell)
            StartCoroutine(spellcaster.Cast(spell, transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>() * speed;
    }

    void Die()
    {
        Debug.Log("You Lost");

        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }

}
