using UnityEngine;
using System.Collections.Generic;

public class SpellContext
{
    public float Power;
    public int Wave;

    public Dictionary<string, float> ToDictionary()
    {
        return new Dictionary<string, float>
        {
            { "power", Power },
            { "wave", Wave }
        };
    }
}
