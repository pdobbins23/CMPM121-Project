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

    public Interface ui = null!;

    void Awake()
    {
        if (s_instance != null) throw new InvalidOperationException("Cannot initialize Game multiple times");
        s_instance = this;

        ui = new Interface();
    }

    void Start() { }

    void Update() { }
}

public class Interface
{
    public Interface()
    {
        var root = new InterfaceBlock();
        root.Attach();
    }
}

public class InterfaceBlock : MultiBlock
{
    public InterfaceBlock()
    {
        Add(new SpellListBlock()).At(32, 32, 160, 30);
        Add(new ClassSelectorBlock()).Center(0, 0, 1000, 600);
    }
}

public class SpellListBlock : MultiBlock
{
    public SpellListBlock()
    {
        for (int i = 0; i < 4; i++)
            Add(new SpellBlock()).At(i * (64 + 8), 0, 64, 64);
    }
}

public class SpellBlock : MultiBlock
{
    public SpellBlock()
    {
        Add(new ImageBlock(Sprites.Get("Sprites/UI/box", "tile_0000_0"))).Center(0, 0, 64, 64);
        Add(new RectBlock(0xff0000)).Center(0, 0, 52, 52);
        Add(new ImageBlock(Sprites.Get("Sprites/Tiles/ProjectUtumno_full", "ProjectUtumno_full_1910"))).Center(0, 0, 48, 48);
        Add(new RectBlock(0x888888, 0.5f)).Center(0, 0, 48, 48);
        Add(new RectBlock(0xffffff)).Center(12, -16, 24, 16);
        Add(new RectBlock(0xffffff)).Center(-12, 16, 24, 16);
        Add(new TextBlock(Util.FormatShort(10), 0x0000ff)).Center(12, -16, 24, 12);
        Add(new TextBlock(Util.FormatShort(20), 0xff0000)).Center(-12, 16, 24, 12);
    }
}

public class ClassSelectorBlock : MultiBlock
{
    private readonly List<string> _classes = new() { "Mage", "Warlock", "Battlemage" };

    public ClassSelectorBlock()
    {
        Add(new PanelBlock()).Center(0, 0, 1000, 600);

        float offset = (_classes.Count - 1) / 2f;

        Add(new TextBlock("Class Selector", 0x333333)).Center(0, (offset + 1) * 60, 320, 32);
        for (int i = 0; i < _classes.Count; i++)
            Add(new ButtonBlock(_classes[i])).Center(0, (offset - i) * 60, 160, 32);
    }
}

public class ButtonBlock : MultiBlock
{
    private readonly Image _image;
    private readonly Button _button;

    private readonly TextBlock _text;

    public ButtonBlock(string text)
    {
        _image = go.AddComponent<Image>();
        _image.sprite = Sprites.Get("Sprites/UI/button", "button");
        _image.type = Image.Type.Sliced;

        _button = go.AddComponent<Button>();

        _text = new TextBlock(text, 0x333333);
        Add(_text).Center(0, 0);
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
