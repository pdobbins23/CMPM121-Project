using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public Level currentLevel;
    public PlayerController player;
    public GameObject enemy;

    void Start() { }

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            GameManager.Instance.currentWave++;

            if (currentLevel.waves == -1 || GameManager.Instance.currentWave < currentLevel.waves) {
                GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
                // continueBtn.gameObject.SetActive(false);

                NextWave();
            } else {
                GameManager.Instance.state = GameManager.GameState.GAMEOVER;
            }
        }
    }

    public void StartLevel(string levelname)
    {
        GameManager.Instance.currentWave = 1;
        // level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        currentLevel = LevelManager.Instance.levelTypes[levelname];
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        var pc = GameManager.Instance.player.GetComponent<PlayerController>();

        // pc.hp.hp = pc.hp.max_hp = 95 + GameManager.Instance.currentWave * 5;
        // pc.spellcaster.mana = 90 + GameManager.Instance.currentWave * 10;
        // pc.spellcaster.mana_reg = GameManager.Instance.currentWave + 10;
        // pc.spellcaster.spell_power = GameManager.Instance.currentWave * 10;
        // pc.speed = 5;

        var ctx = new Dictionary<string, float>
        {
            { "wave", GameManager.Instance.currentWave },
        };

        pc.hp.hp = pc.hp.max_hp = RPN.EvalInt(pc.playerClass.health, ctx);
        pc.spellcaster.mana = RPN.EvalInt(pc.playerClass.mana, ctx);
        pc.spellcaster.mana_reg = RPN.EvalInt(pc.playerClass.mana_regeneration, ctx);
        pc.spellcaster.spell_power = RPN.EvalInt(pc.playerClass.spellpower, ctx);
        pc.speed = RPN.EvalInt(pc.playerClass.speed, ctx);

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;

        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }

        GameManager.Instance.state = GameManager.GameState.INWAVE;

        GameManager.Instance.totalEnemiesForWave = 0;

        foreach (var spawn in currentLevel.spawns) {
            var vars = new Dictionary<string, float> {
                { "wave", (float)GameManager.Instance.currentWave },
            };

            int totalCount = RPN.EvalInt(spawn.count, vars);

            GameManager.Instance.totalEnemiesForWave += totalCount;

            Debug.Log(spawn.enemy + " - " + totalCount);

            while (totalCount > 0) {
                foreach (var c in spawn.sequence) {
                    for (int i = 0; i < c; i++) {
                        if (totalCount == 0)
                            break;

                        totalCount--;

                        Enemy e = EnemyManager.Instance.enemyTypes[spawn.enemy];

                        yield return SpawnEnemy(GameManager.Instance.currentWave, e, spawn);
                    }

                    yield return new WaitForSeconds(spawn.delay);
                }
            }
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);

        GameManager.Instance.state = GameManager.GameState.ENDINGWAVE;

        // continueBtn.gameObject.SetActive(true);

        PlayerController pc = GameManager.Instance.player.GetComponent<PlayerController>();

        Spell newSpell = new SpellBuilder().GetRandomSpell(pc.spellcaster);
        // spellRewardUI.GetComponent<SpellRewardUI>().Show(newSpell, pc.spellcaster);
    }

    IEnumerator SpawnEnemy(int wave, Enemy e, Level.Spawn s)
    {
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;

        Vector3 initial_position = spawnPoint.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(e.sprite);

        var hpVars = new Dictionary<string, float> {
            { "base", e.hp },
            { "wave", wave },
        };

        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(RPN.EvalInt(s.hp, hpVars), Hittable.Team.MONSTERS, new_enemy);
        en.speed = e.speed;

        GameManager.Instance.AddEnemy(new_enemy);

        yield return new WaitForSeconds(0.2f);
    }
}
