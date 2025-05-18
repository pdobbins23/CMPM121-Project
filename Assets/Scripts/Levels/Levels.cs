using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class Level {
    public class Spawn{
        public string enemy;

        public string count;

        public string hp = "base";

        public int delay = 2;

        public int[] sequence = {1};

        public string location = "random";
    }

    public string name;
    public int waves = -1;
    public Spawn[] spawns;
}
