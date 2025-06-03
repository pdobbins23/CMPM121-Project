#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Block
{
    public readonly GameObject go;

    public Block()
    {
        string name = GetType().Name;
        if (name.EndsWith("Block") && name != "MultiBlock") name = name[..^5];
        go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
    }

    public void Set(object data)
    {
        throw new NotImplementedException("Set must be overriden");
    }

    public Block At(float x, float y, float? w = null, float? h = null)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 0);
        if (w != null && h != null)
        {
            Sized(w.Value, h.Value);
        }
        return this;
    }

    public Block Center(float x, float y, float? w = null, float? h = null)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        if (w != null && h != null)
        {
            Sized(w.Value, h.Value);
        }
        return this;
    }

    public virtual Block Sized(float w, float h)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(w, h);
        return this;
    }
}

public class MultiBlock : Block
{
    private readonly List<Block> _children = new();

    public MultiBlock() { }

    public Block Add(Block child)
    {
        child.go.transform.SetParent(go.transform, false);
        _children.Add(child);
        return child;
    }

    public void Attach()
    {
        Canvas canvas = GameObject.FindAnyObjectByType<Canvas>();
        go.transform.SetParent(canvas.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

public class ImageBlock : Block
{
    private readonly Image _image;

    public ImageBlock(Sprite sprite)
    {
        _image = go.AddComponent<Image>();
        Set(sprite);
    }

    public new void Set(object data)
    {
        if (data is Sprite sprite)
        {
            _image.sprite = sprite;
        }
        else
        {
            base.Set(data);
        }
    }
}

public class RectBlock : Block
{
    private readonly Image _image;

    public RectBlock(int color)
    {
        _image = go.AddComponent<Image>();
        Set(color);
    }

    public RectBlock(int color, float alpha)
    {
        _image = go.AddComponent<Image>();
        Set(color);
        var c = _image.color;
        c.a = Mathf.Clamp01(alpha);
        _image.color = c;
    }

    public new void Set(object data)
    {
        if (data is int color)
        {
            _image.color = new Color32(
                (byte)((color >> 16) & 0xff),
                (byte)((color >> 8) & 0xff),
                (byte)(color & 0xff),
                255
            );
        }
        else
        {
            base.Set(data);
        }
    }
}

public class TextBlock : Block
{
    private readonly TextMeshProUGUI _text;

    public TextBlock(string text, int color)
    {
        _text = go.AddComponent<TextMeshProUGUI>();
        _text.alignment = TextAlignmentOptions.Center;
        _text.color = new Color32(
            (byte)((color >> 16) & 0xff),
            (byte)((color >> 8) & 0xff),
            (byte)(color & 0xff),
            255
        );

        Set(text);
    }

    public new void Set(object data)
    {
        if (data is not string str)
        {
            base.Set(data);
            return;
        }

        int leading = str.TakeWhile(c => c == '\t').Count();
        int trailing = str.Reverse().TakeWhile(c => c == '\t').Count();
        string trimmed = str.Trim('\t');

        if (leading > trailing)
            _text.alignment = TextAlignmentOptions.Right;
        else if (trailing > leading)
            _text.alignment = TextAlignmentOptions.Left;
        else
            _text.alignment = TextAlignmentOptions.Center;

        _text.text = trimmed;
    }

    public override Block Sized(float w, float h)
    {
        _text.fontSize = h;
        return base.Sized(w, h);
    }
}

public static class Sprites
{
    private static readonly Dictionary<string, Dictionary<string, Sprite>> s_cache = new();

    public static Sprite Get(string tilesetPath, string spriteName)
    {
        if (!s_cache.TryGetValue(tilesetPath, out var spriteMap))
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(tilesetPath);
            spriteMap = sprites.ToDictionary(s => s.name);
            s_cache[tilesetPath] = spriteMap;
        }

        return spriteMap.TryGetValue(spriteName, out var sprite)
            ? sprite
            : throw new KeyNotFoundException($"Sprite '{spriteName}' not found in '{tilesetPath}'");
    }
}

