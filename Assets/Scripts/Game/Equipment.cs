#nullable enable

using UnityEngine;
using System.Text;

public class Equipment
{
    private readonly string _name;
    public readonly int Sprite;
    public readonly int Type;

    public int max_hp;
    public int max_mana;
    public int mana_reg;
    public int spell_power;
    public int speed;

    public Equipment()
    {
        float roll = Random.value;
        int rarityIndex;
        string rarity;
        if (roll < 0.15f)
        {
            rarityIndex = 2;
            rarity = "Legendary";
        }
        else if (roll < 0.5f)
        {
            rarityIndex = 1;
            rarity = "Rare";
        }
        else
        {
            rarityIndex = 0;
            rarity = "Common";
        }

        // Roll type
        Type = Random.Range(0, 3);
        string typeStr = Type switch
        {
            0 => "Hat",
            1 => "Armor",
            2 => "Boots",
            _ => "Unknown"
        };

        _name = $"{rarity} {typeStr}";

        // Sprite table
        int[,] spriteTable = new int[3, 3]
        {
            { 5931, 5933, 5935 },
            { 5982, 5986, 5981 },
            { 6007, 6005, 6006 },
        };
        Sprite = spriteTable[Type, rarityIndex];

        for (int i = 0; i <= rarityIndex; i++)
        {
            switch (Random.Range(0, 5))
            {
                case 0: max_hp += Random.Range(20, 40); break;
                case 1: max_mana += Random.Range(20, 40); break;
                case 2: mana_reg += Random.Range(2, 8); break;
                case 3: spell_power += Random.Range(10, 60); break;
                case 4: speed += Random.Range(1, 4); break;
            }
        }
    }

    public string GetName()
    {
        return _name;
    }

    public string GetDescription()
    {
        var sb = new StringBuilder();
        if (max_hp > 0) sb.AppendLine($"Max HP: +{max_hp}");
        if (max_mana > 0) sb.AppendLine($"Max Mana: +{max_mana}");
        if (mana_reg > 0) sb.AppendLine($"Mana Regen: +{mana_reg}");
        if (spell_power > 0) sb.AppendLine($"Spell Power: +{spell_power}");
        if (speed > 0) sb.AppendLine($"Speed: +{speed}");
        return sb.ToString().TrimEnd();
    }
}
