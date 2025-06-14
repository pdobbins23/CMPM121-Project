#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public virtual void Refresh() { }

    public Block At(float x, float y, float? w = null, float? h = null)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 0);
        if (w != null && h != null) Sized(w.Value, h.Value);
        return this;
    }

    public Block Center(float x, float y, float? w = null, float? h = null)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        if (w != null && h != null) Sized(w.Value, h.Value);
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

    protected Block Add(Block child)
    {
        child.go.transform.SetParent(go.transform, false);
        _children.Add(child);
        return child;
    }

    public override void Refresh()
    {
        foreach (var block in _children)
            block.Refresh();
    }

    protected void Clear()
    {
        foreach (var block in _children)
            GameObject.Destroy(block.go);
        _children.Clear();
    }

    protected void Remove(Block child)
    {
        GameObject.Destroy(child.go);
        _children.Remove(child);
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

public class EventBlock : MultiBlock
{
    private class EventRelay : MonoBehaviour,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        public EventBlock? owner;

        public void OnPointerClick(PointerEventData ev) => owner?.OnPointerClick(ev);
        public void OnPointerEnter(PointerEventData ev) => owner?.OnPointerEnter(ev);
        public void OnPointerExit(PointerEventData ev) => owner?.OnPointerExit(ev);
        public void OnPointerDown(PointerEventData ev) => owner?.OnPointerDown(ev);
        public void OnPointerUp(PointerEventData ev) => owner?.OnPointerUp(ev);
    }

    public EventBlock()
    {
        go.AddComponent<EventRelay>().owner = this;
    }

    public virtual void OnPointerClick(PointerEventData ev) { }
    public virtual void OnPointerEnter(PointerEventData ev) { }
    public virtual void OnPointerExit(PointerEventData ev) { }
    public virtual void OnPointerDown(PointerEventData ev) { }
    public virtual void OnPointerUp(PointerEventData ev) { }
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
    private readonly float? _fontSizeOverride;

    public TextBlock(string text, int color, float? fontSizeOverride = null)
    {
        _text = go.AddComponent<TextMeshProUGUI>();
        _text.color = new Color32(
            (byte)((color >> 16) & 0xff),
            (byte)((color >> 8) & 0xff),
            (byte)(color & 0xff),
            255
        );

        _fontSizeOverride = fontSizeOverride;

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
        _text.fontSize = _fontSizeOverride ?? h;
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

