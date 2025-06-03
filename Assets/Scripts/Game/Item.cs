#nullable enable

public class Item
{
    public readonly Spell? Spell;
    public readonly Relic? Relic;
    // public readonly Equipment _equipment;

    public Item(Spell spell)
    {
        Spell = spell;
    }

    public Item(Relic relic)
    {
        Relic = relic;
    }

    // public Item(Equipment equipment)
    // {
    //     _equipment = equipment;
    // }
}

public class ItemSlot
{
    public Item? Item;
    public bool Highlighted = false;

    public ItemSlot() { }

    public ItemSlot(Item item)
    {
        Item = item;
    }
}
