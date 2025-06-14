#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    private WaveStatusBlock? _waveStatus;
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

        if (_waveStatus == null)
        {
            _waveStatus = new WaveStatusBlock();
            Add(_waveStatus).Center(0, 465, 800, 36);
        }
        _waveStatus.Refresh();

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
            PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();

            var rewardSpell = new SpellBuilder().GetRandomSpell(pc.spellcaster);
            var rewardRelics = new List<Relic>();

            var rnd = new System.Random();
            var ar = RelicManager.Instance.AllRelics;
            var r = RelicManager.Instance.ActiveRelics;

            var availableRelics = ar
                .Where(data => !r.Any(active => active.Name == data.Name))
                .ToList();

            for (int i = 0; i < 3; i++)
            {
                var eligible = availableRelics
                    .Where(data => !rewardRelics.Any(chosen => chosen.Name == data.Name))
                    .ToList();

                if (eligible.Count() == 0) break;

                rewardRelics.Add(eligible[rnd.Next(eligible.Count())]);
            }

            _rewardMenu = new RewardMenuBlock(_ui, rewardSpell, rewardRelics);
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
    public InventoryBlock() { } // List<ItemSlot> inventory

    public void Refresh()
    {
        Clear();

        if (GameManager.Instance.player == null) return;
        PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();
        if (pc.spellcaster == null) return;

        for (int i = 0; i < 4; i++)
        {
            var slot = new ItemSlot(i < pc.spellcaster.spells.Count ? new Item(pc.spellcaster.spells[i]) : null);
            slot.Highlighted = pc.currentSpell == i;
            Add(new ItemBlock(slot)).At(i * (64 + 8), 0, 64, 64);
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

    public ItemBlock(ItemSlot slot)
    {
        _slot = slot;
    }

    public override Block Sized(float width, float height)
    {
        if (width != height) throw new ArgumentException("Width and height must be equal.");
        float s = width / 64;

        Clear();
        Add(new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"))).Center(0, 0, 64 * s, 64 * s);
        Add(new RectBlock(_slot.Highlighted ? 0xff0000 : 0x000000)).Center(0, 0, 52 * s, 52 * s);

        if (_slot.Item?.Spell is Spell spell)
        {
            Debug.Log($"index={spell.GetIcon()}");
            Debug.Log($"name={_spell_sprites[spell.GetIcon()]}");
            Sprite sprite = Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[spell.GetIcon()]);
            float sinceLast = Time.time - spell.LastCast();
            float ratioLeft = sinceLast > spell.GetCoolDown() ? 0 : 1 - sinceLast / spell.GetCoolDown();

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
        else
        {
            Add(new RectBlock(0xfff1d2)).Center(0, 0, 46 * s, 46 * s);
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

    public void Refresh()
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
    private readonly Spell rewardSpell;
    private readonly List<Relic> rewardRelics;

    private readonly CraftingMenuBlock craftingMenu;

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

    public RewardMenuBlock(Interface ui, Spell rewardSpell, List<Relic> rewardRelics)
    {
        this.rewardSpell = rewardSpell;
        this.rewardRelics = rewardRelics;

        Add(new PanelBlock()).Center(0, 0, 1000, 800);

        Add(new ButtonBlock("Craft", (obj) => {
            craftingMenu.Refresh();
            craftingMenu.go.SetActive(true);
        })).Center(-350, 300, 160, 64);

        Add(new TextBlock("Pick your rewards:", 0x333333)).Center(0, 325, 320, 32);

        // Add(new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"))).Center(0, 175, 200, 200);
        Add(new TextBlock(rewardSpell.GetName(), 0x333333)).Center(0, 50, 750, 32);
        // Sprite rewardSpellSprite = Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[rewardSpell.GetIcon()]);
        // Add(new ImageBlock(rewardSpellSprite)).Center(0, 175, 175, 175);
        Add(new ItemBlock(new ItemSlot(new Item(rewardSpell)))).Center(0, 175, 200, 200);

        var acceptBtn = Add(new ButtonBlock("Accept", (obj) => {
            var pc = GameManager.Instance.player.GetComponent<PlayerController>();

            if (pc.spellcaster.spells.Count < 4)
            {
                pc.spellcaster.spells.Add(rewardSpell);
                obj.go.SetActive(false);
            }

            Debug.Log("spells:");
            Debug.Log(JsonUtility.ToJson(pc.spellcaster.spells));
            // foreach (Spell s in pc.spellcaster.spells)
            //     Debug.Log($"" rawSpell);

        })).Center(0, 10, 160, 32);

        var takeRelicButtons = new List<Block>();

        for (int i = 0; i < rewardRelics.Count(); i++) {
            var rewardRelic = rewardRelics[i];

            Add(new ItemBlock(new ItemSlot(new Item(rewardRelic)))).Center(250 * (i - 1), -100, 100, 100);

            Add(new TextBlock(rewardRelic.Name, 0x333333)).Center(250 * (i - 1), -190, 300, 32);

            var btn = Add(new ButtonBlock("Take", (obj) => {
                foreach (var btn in takeRelicButtons)
                    btn.go.SetActive(false);

                RelicManager.Instance.ActiveRelics.Add(rewardRelic);
            })).Center(250 * (i - 1), -230, 160, 32);

            takeRelicButtons.Add(btn);
        }

        Add(new ButtonBlock("Continue", (obj) =>
            GameManager.Instance.state = GameManager.GameState.WAVEEND
        )).Center(0, -300, 160, 32);

        craftingMenu = (CraftingMenuBlock)Add(new CraftingMenuBlock(ui)).Center(0, 0, 1000, 800);
        craftingMenu.go.SetActive(false);
    }
}

public class CraftingMenuBlock : MultiBlock
{
    struct SpellItem
    {
        public Spell? spell;
        public ImageBlock icon;

        public SpellItem(Spell? spell, ImageBlock icon)
        {
            this.spell = spell;
            this.icon = icon;
        }
    }

    private List<SpellItem> _items = new();

    private SpellItem _item_a = new SpellItem(null, new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0")));
    private SpellItem _item_b = new SpellItem(null, new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0")));

    private Block _item_a_rmbtn;
    private Block _item_b_rmbtn;

    private Block _craft_btn;

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

    public CraftingMenuBlock(Interface ui) {
        Add(new PanelBlock()).Center(0, 0, 1000, 800);

        Add(_item_a.icon).Center(-200, 0, 200, 200);
        Add(_item_b.icon).Center(200, 0, 200, 200);

        _item_a_rmbtn = Add(new ButtonBlock("Remove", (obj) => {
            _craft_btn.go.SetActive(false);
            _item_a_rmbtn.go.SetActive(false);
            // TODO: Actually reset the thing
        })).Center(-200, 180, 100, 32);
        _item_a_rmbtn.go.SetActive(false);

        _item_b_rmbtn = Add(new ButtonBlock("Remove", (obj) => {
            _craft_btn.go.SetActive(false);
            _item_b_rmbtn.go.SetActive(false);
            // TODO: Actually reset the thing
        })).Center(200, 180, 100, 32);
        _item_b_rmbtn.go.SetActive(false);

        _craft_btn = Add(new ButtonBlock("Craft", (obj) => {

        })).Center(0, -150, 200, 50);
        _craft_btn.go.SetActive(false);

        Add(new ButtonBlock("Close", (obj) => {
            go.SetActive(false);
        })).Center(0, -300, 160, 32);
    }

    public void Refresh()
    {
        foreach (var spellItem in _items)
            GameObject.Destroy(spellItem.icon.go);
        _items = new();

        _item_a_rmbtn.go.SetActive(false);
        _item_b_rmbtn.go.SetActive(false);

        var pc = GameManager.Instance.player.GetComponent<PlayerController>();
        var spells = pc.spellcaster.spells;

        for (int idx = 0; idx < spells.Count(); idx++)
        {
            int i = idx;

            var spell = spells[i];
            var item = new SpellItem(spell, new ImageBlock(Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[spell.GetIcon()])));

            _items.Add(item);

            var icon = Add(item.icon).Center(150 * (i - 1), 300, 100, 100);

            Add(new ButtonBlock("Select", (obj) => {
                if (_item_a.spell == null)
                {
                    GameObject.Destroy(_item_a.icon.go);
                    GameObject.Destroy(_items[i].icon.go);

                    _item_a = new SpellItem(spell, new ImageBlock(Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[spell.GetIcon()])));
                    Add(_item_a.icon).Center(-200, 0, 200, 200);

                    _items[i] = new SpellItem(null, new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0")));
                    Add(_items[i].icon).Center(150 * (i - 1), 300, 100, 100);

                    obj.go.SetActive(false);

                    _item_a_rmbtn.go.SetActive(true);
                }
                else if (_item_b.spell == null)
                {
                    GameObject.Destroy(_item_b.icon.go);
                    GameObject.Destroy(_items[i].icon.go);

                    _item_b = new SpellItem(spell, new ImageBlock(Sprites.Get("Sprites/Tiles/ProjectUtumno_full", _spell_sprites[spell.GetIcon()])));
                    Add(_item_b.icon).Center(200, 0, 200, 200);

                    _items[i] = new SpellItem(null, new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0")));
                    Add(_items[i].icon).Center(150 * (i - 1), 300, 100, 100);

                    obj.go.SetActive(false);

                    _item_b_rmbtn.go.SetActive(true);

                    _craft_btn.go.SetActive(true);
                }
            })).Center(150 * (i - 1), 225, 100, 32);
        }

        if (_item_a.spell != null)
        {
            GameObject.Destroy(_item_a.icon.go);

            _item_a.spell = null;
            _item_a.icon = new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"));

            Add(_item_a.icon).Center(-200, 0, 200, 200);
        }
        if (_item_b.spell != null)
        {
            GameObject.Destroy(_item_b.icon.go);

            _item_b.spell = null;
            _item_b.icon = new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"));

            Add(_item_b.icon).Center(200, 0, 200, 200);
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
