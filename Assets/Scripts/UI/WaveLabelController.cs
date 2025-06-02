using UnityEngine;
using TMPro;

public class WaveLabelController : MonoBehaviour
{
    TextMeshProUGUI tmp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.Instance.state)
        {
            case GameManager.GameState.INWAVE:
                tmp.text = "Enemies left: " + GameManager.Instance.enemy_count;
                break;
            case GameManager.GameState.COUNTDOWN:
                tmp.text = "Starting in " + GameManager.Instance.countdown;
                break;
            case GameManager.GameState.ENDINGWAVE:
                tmp.text = "Total Enemies Killed: " + GameManager.Instance.totalEnemiesForWave;
                break;
            case GameManager.GameState.GAMEOVER:
                tmp.text = "GAME OVER";
                break;
            default:
                break;
        }
    }
}
