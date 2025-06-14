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

        var ev = new Evaluator(new() { { "wave", GameManager.Instance.currentWave } });

        pc.hp.max_hp = ev.EvaluateInt(pc.playerClass.health);
        pc.spellcaster.mana = ev.EvaluateInt(pc.playerClass.mana);
        pc.spellcaster.mana_reg = ev.EvaluateInt(pc.playerClass.mana_regeneration);
        pc.spellcaster.spell_power = ev.EvaluateInt(pc.playerClass.spellpower);
        pc.speed = ev.EvaluateInt(pc.playerClass.speed);

        foreach (var slot in pc.Equipments)
            if (slot.Item?.Equipment is Equipment equipment)
            {
                pc.hp.max_hp += equipment.max_hp;
                pc.spellcaster.mana += equipment.mana;
                pc.spellcaster.mana_reg += equipment.mana_reg;
                pc.spellcaster.spell_power += equipment.spell_power;
                pc.speed += equipment.speed;
            }

        pc.hp.hp = pc.hp.max_hp;

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
            var ev = new Evaluator(new() { { "wave", (float)GameManager.Instance.currentWave } });

            int totalCount = ev.EvaluateInt(spawn.count);

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

        EnemyController en = new_enemy.GetComponent<EnemyController>();

        var ev = new Evaluator(new() { { "base", e.hp }, { "wave", wave } });

        en.hp = new Hittable(ev.EvaluateInt(s.hp), Hittable.Team.MONSTERS, new_enemy);
        en.speed = e.speed;

        GameManager.Instance.AddEnemy(new_enemy);

        yield return new WaitForSeconds(0.2f);
    }
}
