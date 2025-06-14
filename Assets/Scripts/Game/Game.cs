#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Game : MonoBehaviour
{
    public Interface? ui = null!;

    void Awake()
    {
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

    public string? home_overlay = "home";

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
    private WaveStatusBlock? _waveStatus;
    private ClassMenuBlock? _classMenu;
    private LevelMenuBlock? _levelMenu;
    private RewardMenuBlock? _rewardMenu;
    private HomeBlock? _home;

    public InterfaceBlock(Interface ui)
    {
        _ui = ui;
        Refresh();
    }

    public override void Refresh()
    {
        if (_inventory == null)
        {
            _inventory = new InventoryBlock();
            Add(_inventory).At(32, 32, 160, 30);
        }

        if (_waveStatus == null)
        {
            _waveStatus = new WaveStatusBlock();
            Add(_waveStatus).Center(0, 465, 800, 36);
        }

        if (_classMenu == null && _ui.class_ == null)
        {
            _classMenu = new ClassMenuBlock(_ui);
            Add(_classMenu).Center(0, 0, 1000, 600);
        }
        else if (_classMenu != null && _ui.class_ != null)
        {
            Remove(_classMenu);
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
            Remove(_levelMenu);
            _levelMenu = null;

            _ui.wm.StartLevel(_ui.level);
        }

        if (_home == null && _ui.home_overlay != null)
        {
            _home = new HomeBlock(_ui);
            Add(_home).Center(0, 0, 1200, 700);
        }
        else if (_home != null && _ui.home_overlay == null)
        {
            Remove(_home);
            _home = null;
        }

        if (_rewardMenu == null && GameManager.Instance.state == GameManager.GameState.ENDINGWAVE)
        {
            PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();

            var rewardSpell = new SpellBuilder().GetRandomSpell(pc.spellcaster);
            var rewardRelics = new List<Relic>();

            var rnd = new System.Random();
            var ar = RelicManager.Instance.AllRelics;
            var r = RelicManager.Instance.ActiveRelics;

            var availableRelics = ar
                .Where(data => !r.Any(active => active.GetName() == data.GetName()))
                .ToList();

            for (int i = 0; i < 3; i++)
            {
                var eligible = availableRelics
                    .Where(data => !rewardRelics.Any(chosen => chosen.GetName() == data.GetName()))
                    .ToList();

                if (eligible.Count() == 0) break;

                rewardRelics.Add(eligible[rnd.Next(eligible.Count())]);
            }

            _rewardMenu = new RewardMenuBlock(_ui, rewardSpell, rewardRelics, new Equipment());
            Add(_rewardMenu).Center(0, 0, 1000, 800);
        }
        else if (_rewardMenu != null && GameManager.Instance.state != GameManager.GameState.ENDINGWAVE)
        {
            Remove(_rewardMenu);
            _rewardMenu = null;
        }

        base.Refresh();
    }
}

public class InventoryBlock : MultiBlock
{
    List<ItemSlot> _inventory = new();

    public override void Refresh()
    {
        var pc = GameManager.Instance.player?.GetComponent<PlayerController>();
        var inventory = pc?.Inventory ?? new();
        if (_inventory.SequenceEqual(inventory))
        {
            base.Refresh();
            return;
        }

        Clear();

        _inventory = inventory.ToList();

        for (int i = 0; i < _inventory.Count; i++)
        {
            Add(new ItemBlock(_inventory[i])).At(i * (64 + 8), 0, 64, 64);
        }
        for (int i = 0; i < pc!.Equipments.Count; i++)
        {
            Add(new ItemBlock(pc.Equipments[2 - i])).At(0, 96 + i * (64 + 8), 64, 64);
        }
    }
}

public class ItemBlock : EventBlock
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
    private readonly List<string> _relic_sprites = new() {
        "ProjectUtumno_full_823",
        "ProjectUtumno_full_865",
        "ProjectUtumno_full_1879",
        "ProjectUtumno_full_1896",
        "ProjectUtumno_full_1897",
        "ProjectUtumno_full_2220",
        "ProjectUtumno_full_2237",
        "ProjectUtumno_full_2238",
        "ProjectUtumno_full_2239",
        "ProjectUtumno_full_2287",
        "ProjectUtumno_full_2549",
        "ProjectUtumno_full_2569",
        "ProjectUtumno_full_2617",
        "ProjectUtumno_full_2620",
        "ProjectUtumno_full_2770",
        "ProjectUtumno_full_5417",
    };

    private readonly ItemSlot _slot;

    private bool hovered;
    private bool shift_up;

    public ItemBlock(ItemSlot slot)
    {
        _slot = slot;
    }

    public override void Refresh()
    {
        float size = go.GetComponent<RectTransform>().sizeDelta.x;
        float s = size / 64;

        var pc = GameManager.Instance.player.GetComponent<PlayerController>();

        Clear();
        Add(new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"))).Center(0, 0, 64 * s, 64 * s);
        Add(new RectBlock(pc.ActiveSlot == _slot ? 0xff0000 : 0x000000)).Center(0, 0, 52 * s, 52 * s);

        if (_slot.Item?.Spell is Spell spell)
        {
            Sprite sprite = Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[spell.GetIcon()]);
            float sinceLast = Time.time - spell.LastCast();
            float ratioLeft = sinceLast > spell.GetCooldown() ? 0 : 1 - sinceLast / spell.GetCooldown();

            Add(new ImageBlock(sprite)).Center(0, 0, 48 * s, 48 * s);
            Add(new RectBlock(0x888888, 0.5f)).Center((-24 + 24 * ratioLeft) * s, 0, 48 * ratioLeft * s, 48 * s);
            Add(new RectBlock(0xffffff)).Center(12 * s, -16 * s, 24 * s, 16 * s);
            Add(new RectBlock(0xffffff)).Center(-12 * s, 16 * s, 24 * s, 16 * s);
            Add(new TextBlock(Util.FormatShort(spell.GetManaCost()), 0x0000ff)).Center(12 * s, -16 * s, 24 * s, 12 * s);
            Add(new TextBlock(Util.FormatShort((int) spell.GetBaseDamage()), 0xff0000)).Center(-12 * s, 16 * s, 24 * s, 12 * s);
        }
        else if (_slot.Item?.Relic is Relic relic)
        {
            Sprite sprite = Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _relic_sprites[relic.GetIcon()]);

            Add(new ImageBlock(sprite)).Center(0, 0, 48 * s, 48 * s);
        }
        else if (_slot.Item?.Equipment is Equipment equipment)
        {
            Sprite sprite = Sprites.Get("Sprites/Tiles/ProjectUtumno_full", $"ProjectUtumno_full_{equipment.Sprite}");

            Add(new ImageBlock(sprite)).Center(0, 0, 48 * s, 48 * s);
        }
        else
        {
            Add(new RectBlock(_slot.AllowPut ? 0xfff1d2 : 0xc2bbae)).Center(0, 0, 46 * s, 46 * s);
        }

        if (_slot.Item != null && hovered)
        {
            var tooltip = Add(new TooltipBlock(_slot));
            if (shift_up)
                tooltip.At(0, size + 20, 500, 300);
            else
                tooltip.At(size + 20, size - 400, 500, 300);
        }
    }

    public override void OnPointerEnter(PointerEventData ev)
    {
        hovered = true;
        shift_up = ev.position.y < 400;
    }

    public override void OnPointerExit(PointerEventData ev)
    {
        hovered = false;
    }

    public override void OnPointerClick(PointerEventData ev)
    {
        var pc = GameManager.Instance.player.GetComponent<PlayerController>();
        if (pc.ActiveSlot == _slot)
            pc.ActiveSlot = new ItemSlot(null, false);
        else if (pc.ActiveSlot.Item != null && (_slot.Item == null && _slot.CanPut(pc.ActiveSlot.Item)))
        {
            _slot.Item = pc.ActiveSlot.Item;
            pc.ActiveSlot.Item = null;
            pc.ActiveSlot = new ItemSlot(null, false);
        }
        else
            pc.ActiveSlot = _slot;
    }
}

public class TooltipBlock : EventBlock
{
    private readonly ItemSlot _slot;

    public TooltipBlock(ItemSlot slot)
    {
        var image = go.AddComponent<Image>();
        image.sprite = Sprites.Get("Sprites/UI/panel", "tile_0001_0");
        image.type = Image.Type.Tiled;

        var canvas = go.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        _slot = slot;
    }

    public override Block Sized(float width, float height)
    {
        Clear();

        if (_slot.Item?.Spell is Spell spell)
        {
            Add(new TextBlock(spell.GetName(), 0xffffff, height * 0.1f)).At(0, height * 0.65f, width, height * 0.35f);
            Add(new TextBlock(spell.GetDescription(), 0xffffff, height * 0.07f)).At(0, 0, width, height * 0.7f);
        }
        else if (_slot.Item?.Relic is Relic relic)
        {
            Add(new TextBlock(relic.GetName(), 0xffffff, height * 0.1f)).At(0, height * 0.65f, width, height * 0.35f);
            Add(new TextBlock(relic.GetDescription(), 0xffffff, height * 0.07f)).At(0, 0, width, height * 0.7f);
        }
        else if (_slot.Item?.Equipment is Equipment equipment)
        {
            Add(new TextBlock(equipment.GetName(), 0xffffff, height * 0.1f)).At(0, height * 0.65f, width, height * 0.35f);
            Add(new TextBlock(equipment.GetDescription(), 0xffffff, height * 0.07f)).At(0, 0, width, height * 0.7f);
        }

        return base.Sized(width, height);
    }
}

public class WaveStatusBlock : MultiBlock
{
    private readonly TextBlock _text;

    public WaveStatusBlock()
    {
        _text = new TextBlock("", 0xffffff);
        Add(_text).Center(0, 0, 800, 36);
    }

    public override void Refresh()
    {
        switch (GameManager.Instance.state)
        {
            case GameManager.GameState.INWAVE:
                _text.Set($"Enemies Left: {GameManager.Instance.enemy_count}");
                break;
            case GameManager.GameState.COUNTDOWN:
                _text.Set($"Starting in {GameManager.Instance.countdown}...");
                break;
            case GameManager.GameState.ENDINGWAVE:
                _text.Set($"Total Enemies Killed: {GameManager.Instance.totalEnemiesForWave}");
                break;
            case GameManager.GameState.GAMEOVER:
                _text.Set("GAME OVER");
                break;
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
            Add(new ButtonBlock(class_, (obj) => ui.class_ = class_)).Center(0, (offset - i) * 60, 160, 32);
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
            Add(new ButtonBlock(level, (obj) => ui.level = level)).Center(0, (offset - i) * 60, 160, 32);
        }
    }
}

public class RewardMenuBlock : MultiBlock
{
    private readonly CraftingMenuBlock craftingMenu;

    private readonly List<ItemSlot> relicItemSlots = new();

    public RewardMenuBlock(Interface ui, Spell rewardSpell, List<Relic> rewardRelics, Equipment rewardEquipment)
    {
        Add(new PanelBlock()).Center(0, 0, 1000, 800);

        Add(new ButtonBlock("Craft", (obj) => {
            craftingMenu?.go.SetActive(true);
        })).Center(-350, 300, 160, 64);

        Add(new TextBlock("Grab your rewards:", 0x333333)).Center(0, 325, 320, 32);

        Add(new ItemBlock(new ItemSlot(new Item(rewardSpell), false))).Center(0, 175, 200, 200);
        Add(new TextBlock(rewardSpell.GetName(), 0x333333)).Center(0, 50, 750, 32);

        for (int i = 0; i < rewardRelics.Count; i++) {
            var rewardRelic = rewardRelics[i];
            var slot = new ItemSlot(new Item(rewardRelic), false);
            relicItemSlots.Add(slot);
            Add(new ItemBlock(slot)).Center(200 * (i - 1.5f), -100, 100, 100);
            Add(new TextBlock(rewardRelic.GetName(), 0x333333)).Center(200 * (i - 1.5f), -190, 300, 32);
        }

        Add(new ItemBlock(new ItemSlot(new Item(rewardEquipment), false))).Center(200 * 1.5f, -100, 100, 100);
        Add(new TextBlock(rewardEquipment.GetName(), 0x333333)).Center(200 * 1.5f, -190, 300, 32);

        Add(new ButtonBlock("Continue", (obj) =>
            GameManager.Instance.state = GameManager.GameState.WAVEEND
        )).Center(0, -300, 160, 32);

        craftingMenu = (CraftingMenuBlock)Add(new CraftingMenuBlock(ui)).Center(0, 0, 1000, 800);
        craftingMenu.go.SetActive(false);
    }

    public override void Refresh()
    {
        if (relicItemSlots.Any(slot => slot.Item == null))
            foreach (var slot in relicItemSlots) slot.Item = null;

        base.Refresh();
    }
}

public class CraftingMenuBlock : MultiBlock
{

    private ItemSlot _item_left = new();
    private ItemSlot _item_right = new();
    private ItemSlot _item_output = new(null, false);

    private bool _has_crafted;

    private Block _close_button;

    public CraftingMenuBlock(Interface ui) {
        Add(new PanelBlock()).Center(0, 0, 1000, 800);

        Add(new ItemBlock(_item_left)).Center(-300, 0, 200, 200);
        Add(new ItemBlock(_item_right)).Center(-50, 0, 200, 200);
        Add(new ItemBlock(_item_output)).Center(300, 0, 200, 200);

        _close_button = Add(new ButtonBlock("Close", (obj) => {
            go.SetActive(false);
        })).Center(0, -300, 160, 32);
    }

    public override void Refresh()
    {
        if (_has_crafted)
        {
            if (_item_output.Item == null)
            {
                _item_left.Item = null;
                _item_right.Item = null;
            }
            if (_item_left.Item == null || _item_right.Item == null)
            {
                _item_output.Item = null;
                _has_crafted = false;
            }
        }

        if (_item_left.Item?.Spell is Spell a && _item_right.Item?.Spell is Spell b)
        {
            var crafted = RawSpell.CraftSpell(a.GetRaw(), b.GetRaw());
            if (crafted != null)
            {
                var pc = GameManager.Instance.player.GetComponent<PlayerController>();
                _item_output.Item = new Item(new Spell(crafted, pc.spellcaster));
                _has_crafted = true;
            }
        }

        _close_button.go.SetActive(_item_left.Item == null && _item_right.Item == null);

        base.Refresh();
    }
}

public class HomeBlock : MultiBlock
{
    private readonly Interface _ui;
    private string? _home_overlay;
    private bool _refresh;

    public HomeBlock(Interface ui)
    {
        _ui = ui;
        Refresh();
    }

    public override void Refresh()
    {
        if (_home_overlay == _ui.home_overlay && !_refresh) return;
        _home_overlay = _ui.home_overlay;
        _refresh = false;

        Clear();

        Add(new RectBlock(0x876542)).Center(0, 0, 15360, 8640);
        Add(new PanelBlock()).Center(0, 0, 1200, 700);

        switch (_ui.home_overlay)
        {
            case "home":
                Add(new TextBlock("Fang & Fur", 0x7a4e1c)).Center(0, 160, 600, 100);
                Add(new ButtonBlock("Play", _ => _ui.home_overlay = null).Center(0, 0, 200, 50));
                Add(new ButtonBlock("Credits", _ => { _ui.home_overlay = "credits"; Refresh(); }).Center(0, -80, 200, 50));
                Add(new ButtonBlock("Options", _ => { _ui.home_overlay = "options"; Refresh(); }).Center(0, -160, 200, 50));
                break;
            case "credits":
                Add(new ButtonBlock("Back", _ => { _ui.home_overlay = "home"; Refresh(); }).At(60, 60, 200, 50));

                Add(new TextBlock("Credits (Code)", 0x7a4e1c)).Center(-300, 160, 600, 60);
                Add(new TextBlock("Peter Dobbins", 0x7a4e1c)).Center(-300, 80, 600, 30);
                Add(new TextBlock("Astra Tsai", 0x7a4e1c)).Center(-300, 40, 600, 30);
                Add(new TextBlock("Seeya Pillai", 0x7a4e1c)).Center(-300, 0, 600, 30);
                Add(new TextBlock("Anson Fong", 0x7a4e1c)).Center(-300, -40, 600, 30);
                Add(new TextBlock("Peichen Yao", 0x7a4e1c)).Center(-300, -80, 600, 30);
                Add(new TextBlock("Prof. Markus Eger", 0x7a4e1c)).Center(-300, -120, 600, 30);

                Add(new TextBlock("Credits (Art)", 0x7a4e1c)).Center(300, 160, 600, 60);
                Add(new TextBlock("https://opengameart.org/content/dungeon-crawl-32x32-tiles", 0x7a4e1c)).Center(250, 80, 600, 20);
                Add(new TextBlock("https://opengameart.org/content/roguelike-caves-dungeons-pack", 0x7a4e1c)).Center(250, 40, 600, 20);
                Add(new TextBlock("https://kenney.nl/assets/roguelike-caves-dungeons", 0x7a4e1c)).Center(250, 0, 600, 20);
                Add(new TextBlock("https://kenney.nl/assets/ui-pack-pixel-adventure", 0x7a4e1c)).Center(250, -40, 600, 20);
                Add(new TextBlock("https://opengameart.org/content/tiny-creatures", 0x7a4e1c)).Center(250, -80, 600, 20);
                Add(new TextBlock("https://opengameart.org/content/arcane-magic-effect", 0x7a4e1c)).Center(250, -120, 600, 20);

                break;
            case "options":
                Add(new ButtonBlock("Back", _ => { _ui.home_overlay = "home"; Refresh(); }).At(60, 60, 200, 50));

                var am = AudioManager.Instance;
                Add(new TextBlock($"SFX Volume: {Math.Round(am.SfxVolume * 100)}%", 0x7a4e1c)).Center(-150, 80, 300, 30);
                Add(new TextBlock($"Music Volume: {Math.Round(am.MusicVolume * 100)}%", 0x7a4e1c)).Center(-150, 0, 300, 30);
                Add(new ButtonBlock("Mute", _ => { am.SfxVolume = 0; _refresh = true; })).Center(50, 80, 90, 40);
                Add(new ButtonBlock("Mute", _ => { am.MusicVolume = 0; _refresh = true; })).Center(50, 0, 90, 40);
                Add(new ButtonBlock("-", _ => { am.SfxVolume = Math.Clamp(am.SfxVolume - 0.1f, 0, 1); _refresh = true; })).Center(125, 80, 40, 40);
                Add(new ButtonBlock("-", _ => { am.MusicVolume = Math.Clamp(am.MusicVolume - 0.1f, 0, 1); _refresh = true; })).Center(125, 0, 40, 40);
                Add(new ButtonBlock("+", _ => { am.SfxVolume = Math.Clamp(am.SfxVolume + 0.1f, 0, 1); _refresh = true; })).Center(175, 80, 40, 40);
                Add(new ButtonBlock("+", _ => { am.MusicVolume = Math.Clamp(am.MusicVolume + 0.1f, 0, 1); _refresh = true; })).Center(175, 0, 40, 40);

                var colorblindness = HealthBar.ColorBlindMode switch {
                    1 => "Red-Green",
                    2 => "Full",
                    _ => "Off",
                };
                Add(new TextBlock($"Colorblindness Mode: {colorblindness}", 0x7a4e1c)).Center(-225, -80, 450, 30);
                Add(new ButtonBlock("R/G", _ => { HealthBar.ColorBlindMode = 1; _refresh = true; })).Center(50, -80, 90, 40);
                Add(new ButtonBlock("Full", _ => { HealthBar.ColorBlindMode = 2; _refresh = true; })).Center(150, -80, 90, 40);
                Add(new ButtonBlock("Off", _ => { HealthBar.ColorBlindMode = 0; _refresh = true; })).Center(250, -80, 90, 40);

                break;
        }
    }
}

public class ButtonBlock : MultiBlock
{
    private readonly TextBlock _text;
    private int _clicksPending;

    public ButtonBlock(string text, Action<Block>? action = null)
    {
        var image = go.AddComponent<Image>();
        image.sprite = Sprites.Get("Sprites/UI/button", "button");
        image.type = Image.Type.Sliced;

        var button = go.AddComponent<Button>();
        button.onClick.AddListener(() => _clicksPending++);
        if (action != null) button.onClick.AddListener(() => action.Invoke(this));

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
