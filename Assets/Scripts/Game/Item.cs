#nullable enable

public class Item
{
    public readonly Spell _spell;
    public readonly Relic _relic;
    // public readonly Equipment _equipment;

    public Item(Spell spell)
    {
        _spell = spell;
    }

    public Item(Relic relic)
    {
        _relic = relic;
    }

    // public Item(Equipment equipment)
    // {
    //     _equipment = equipment;
    // }
}

public class ItemSlot
{
    public Item? Item;
    public bool highlighted = false;

    public ItemSlot() { }

    public ItemSlot(Item item)
    {
        Item = item;
    }
}
