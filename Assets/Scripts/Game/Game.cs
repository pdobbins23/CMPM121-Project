#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private static Game? s_instance;

    public static Game Get()
    {
        if (s_instance == null) throw new InvalidOperationException("Game not yet ready");
        return s_instance;
    }

    public Interface? ui = null!;

    void Awake()
    {
        if (s_instance != null) throw new InvalidOperationException("Cannot initialize Game multiple times");
        s_instance = this;

        ui = new Interface();
    }

    void Start() { }

    void Update()
    {
        ui?.Update();
    }
}

public class Interface
{
    public WaveManager wm;

    public string? class_;
    public string? level;

    private readonly InterfaceBlock? _root;

    public Interface()
    {
        wm = GameObject.FindAnyObjectByType<WaveManager>();

        _root = new InterfaceBlock(this);
        _root.Attach();
    }

    public void Update()
    {
        _root?.Refresh();
    }
}

public class InterfaceBlock : MultiBlock
{
    private readonly Interface _ui;
    private InventoryBlock? _inventory;
    private ClassMenuBlock? _classMenu;
    private LevelMenuBlock? _levelMenu;
    private RewardMenuBlock? _rewardMenu;

    public InterfaceBlock(Interface ui)
    {
        _ui = ui;
        Refresh();
    }

    public void Refresh()
    {
        if (_inventory == null)
        {
            _inventory = new InventoryBlock();
            Add(_inventory).At(32, 32, 160, 30);
        }
        _inventory.Refresh();

        if (_classMenu == null && _ui.class_ == null)
        {
            _classMenu = new ClassMenuBlock(_ui);
            Add(_classMenu).Center(0, 0, 1000, 600);
        }
        else if (_classMenu != null && _ui.class_ != null)
        {
            GameObject.Destroy(_classMenu.go);
            _classMenu = null;

            PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();
            pc.playerClass = PlayerClassManager.Instance.playerClasses[_ui.class_];
        }

        if (_levelMenu == null && _ui.level == null)
        {
            _levelMenu = new LevelMenuBlock(_ui);
            Add(_levelMenu).Center(0, 0, 1000, 600);
        }
        else if (_levelMenu != null && _ui.level != null)
        {
            GameObject.Destroy(_levelMenu.go);
            _levelMenu = null;

            _ui.wm.StartLevel(_ui.level);
        }

        if (_rewardMenu == null && GameManager.Instance.state == GameManager.GameState.ENDINGWAVE)
        {
            _rewardMenu = new RewardMenuBlock(_ui);
            Add(_rewardMenu).Center(0, 0, 1000, 800);
        }
        else if (_rewardMenu != null && GameManager.Instance.state != GameManager.GameState.ENDINGWAVE)
        {
            GameObject.Destroy(_rewardMenu.go);
            _rewardMenu = null;
        }
    }
}

public class InventoryBlock : MultiBlock
{
    List<ItemBlock> _items = new();
    public InventoryBlock() { } // List<ItemSlot> inventory

    public void Refresh()
    {

        foreach (var item in _items)
            GameObject.Destroy(item.go);
        _items = new();

        if (GameManager.Instance.player == null) return;
        PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();
        if (pc.spellcaster == null) return;

        for (int i = 0; i < 4; i++)
        {
            var slot = new ItemSlot(i < pc.spellcaster.spells.Count ? new Item(pc.spellcaster.spells[i]) : null);
            slot.Highlighted = pc.currentSpell == i;
            var block = new ItemBlock(slot);
            _items.Add(block);
            Add(block).At(i * (64 + 8), 0, 64, 64);
        }
    }
}

public class ItemBlock : MultiBlock
{
    private readonly List<string> _spell_sprites = new() {
        "ProjectUtumno_full_1910",
        "ProjectUtumno_full_1908",
        "ProjectUtumno_full_1915",
        "ProjectUtumno_full_1911",
        "ProjectUtumno_full_1906",
        "ProjectUtumno_full_2002",
        "ProjectUtumno_full_1951",
        "ProjectUtumno_full_1998",
        "ProjectUtumno_full_2005",
        "ProjectUtumno_full_2027",
        "ProjectUtumno_full_2031",
        "ProjectUtumno_full_2037",
        "ProjectUtumno_full_2039",
        "ProjectUtumno_full_2041",
        "ProjectUtumno_full_2130",
        "ProjectUtumno_full_2132",
        "ProjectUtumno_full_2135",
        "ProjectUtumno_full_2198",
    };

    public ItemBlock(ItemSlot slot)
    {
        Add(new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"))).Center(0, 0, 64, 64);
        Add(new RectBlock(slot.Highlighted ? 0xff0000 : 0x000000)).Center(0, 0, 52, 52);

        if (slot.Item == null)
        {
            Add(new RectBlock(0xfff1d2)).Center(0, 0, 46, 46);
        }
        else if (slot.Item.Spell != null)
        {
            Spell spell = slot.Item.Spell;
            Sprite sprite = Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[spell.GetIcon()]);
            float sinceLast = Time.time - spell.LastCast();
            float ratioLeft = sinceLast > spell.GetCoolDown() ? 0 : 1 - sinceLast / spell.GetCoolDown();

            Add(new ImageBlock(sprite)).Center(0, 0, 48, 48);
            Add(new RectBlock(0x888888, 0.5f)).Center(-24 + 24 * ratioLeft, 0, 48 * ratioLeft, 48);
            Add(new RectBlock(0xffffff)).Center(12, -16, 24, 16);
            Add(new RectBlock(0xffffff)).Center(-12, 16, 24, 16);
            Add(new TextBlock(Util.FormatShort(spell.GetManaCost()), 0x0000ff)).Center(12, -16, 24, 12);
            Add(new TextBlock(Util.FormatShort((int) spell.GetBaseDamage()), 0xff0000)).Center(-12, 16, 24, 12);
        }
        else if (slot.Item.Relic != null)
        {
            // TODO
        }
    }
}

public class ClassMenuBlock : MultiBlock
{
    private readonly List<string> _classes = new() { "Mage", "Warlock", "Battlemage" };

    public ClassMenuBlock(Interface ui)
    {
        Add(new PanelBlock()).Center(0, 0, 1000, 600);

        float offset = (_classes.Count - 1) / 2f;

        Add(new TextBlock("Class Selector", 0x333333)).Center(0, (offset + 1) * 60, 320, 32);
        for (int i = 0; i < _classes.Count; i++)
        {
            string class_ = _classes[i];
            Add(new ButtonBlock(class_, () => ui.class_ = class_)).Center(0, (offset - i) * 60, 160, 32);
        }
    }
}

public class LevelMenuBlock : MultiBlock
{
    private readonly List<string> _levels = new() { "Easy", "Medium", "Endless" };

    public LevelMenuBlock(Interface ui)
    {
        Add(new PanelBlock()).Center(0, 0, 1000, 600);

        float offset = (_levels.Count - 1) / 2f;

        Add(new TextBlock("Level Selector", 0x333333)).Center(0, (offset + 1) * 60, 320, 32);
        for (int i = 0; i < _levels.Count; i++)
        {
            string level = _levels[i];
            Add(new ButtonBlock(level, () => ui.level = level)).Center(0, (offset - i) * 60, 160, 32);
        }
    }
}

public class RewardMenuBlock : MultiBlock
{
    private readonly List<string> _classes = new() { "Mage", "Warlock", "Battlemage" };

    public RewardMenuBlock(Interface ui)
    {
        Add(new PanelBlock()).Center(0, 0, 1000, 800);

        float offset = (_classes.Count - 1) / 2f;

        Add(new TextBlock("Pick your rewards:", 0x333333)).Center(0, 300, 320, 32);

        Add(new ButtonBlock("Accept")).Center(0, 40, 160, 32);

        for (int i = 0; i < 3; i++)
            Add(new ButtonBlock("Take")).Center(200 * (i - 1), -190, 160, 32);
        Add(new ButtonBlock("Continue", () =>
            GameManager.Instance.state = GameManager.GameState.WAVEEND
        )).Center(0, -270, 160, 32);
    }
}

public class ButtonBlock : MultiBlock
{
    private readonly TextBlock _text;
    private int _clicksPending;

    public ButtonBlock(string text, Action? action = null)
    {
        var image = go.AddComponent<Image>();
        image.sprite = Sprites.Get("Sprites/UI/button", "button");
        image.type = Image.Type.Sliced;

        var button = go.AddComponent<Button>();
        button.onClick.AddListener(() => _clicksPending++);
        if (action != null) button.onClick.AddListener(() => action.Invoke());

        _text = new TextBlock(text, 0xffffff);
        Add(_text).Center(0, 0);
    }

    public bool ReadClick()
    {
        if (_clicksPending == 0) return false;
        _clicksPending--;
        return true;
    }

    public override Block Sized(float w, float h)
    {
        base.Sized(w, h);
        _text.Sized(w, h * 0.75f);
        return this;
    }
}

public class PanelBlock : MultiBlock
{
    private readonly Image _image;

    public PanelBlock()
    {
        _image = go.AddComponent<Image>();
        _image.sprite = Sprites.Get("Sprites/UI/UIPanel", "UIPanel");
        _image.type = Image.Type.Tiled;
        _image.pixelsPerUnitMultiplier = 0.5f;
    }
}

public static class Util
{
    public static string FormatShort(int value)
    {
        int clamped = Math.Clamp(value, 0, 99999);
        if (clamped < 1000) return clamped.ToString();
        if (clamped < 10000) return (clamped / 1000f).ToString("0.0") + "k";
        return (clamped / 1000).ToString() + "k";
    }
}
