#nullable enable

using System;

public class Item
{
    public readonly Spell? Spell;
    public readonly Relic? Relic;
    public readonly Equipment? Equipment;

    public Item(Spell spell)
    {
        Spell = spell;
    }

    public Item(Relic relic)
    {
        Relic = relic;
    }

    public Item(Equipment equipment)
    {
        Equipment = equipment;
    }
}

public class ItemSlot
{
    public Item? Item;
    public readonly bool AllowPut = true;
    public readonly Func<Item, bool>? _filter;

    public ItemSlot(Item? item = null, bool allow_put = true)
    {
        Item = item;
        AllowPut = allow_put;
        _filter = null;
    }

    public ItemSlot(Item? item, Func<Item, bool> filter)
    {
        Item = item;
        _filter = filter;
    }

    public bool CanPut(Item item)
    {
        return AllowPut && (_filter == null || _filter(item));
    }
}
