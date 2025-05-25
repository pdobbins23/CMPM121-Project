using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        ENDINGWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }
    public GameState state;

    public int countdown;
    public int currentWave = 1;
    public int totalEnemiesForWave = 0;

    
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public event Action OnWaveStart;
    public void StartNextWave() {
        OnWaveStart?.Invoke();
    // wave logic here...
}


    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
    public void KillAllEnemies()
    {
        foreach (var enemy in enemies.ToList())
        {
            var hp = enemy.GetComponent<EnemyController>().hp;
            hp.Damage(new Damage(hp.hp, Damage.Type.DARK));
        }
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    void OnWaveCompleted()
    {
        // PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();
        
        // Spell newSpell = new SpellBuilder().GetRandomSpell(pc.spellcaster);
        // SpellRewardUI.Instance.Show(newSpell, pc.spellcaster);
    }


    private GameManager()
    {
        enemies = new List<GameObject>();
    }
}

