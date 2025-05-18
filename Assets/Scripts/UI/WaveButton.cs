using UnityEngine;
using TMPro;

public class WaveButton : MonoBehaviour
{
    public TextMeshProUGUI label;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        label.text = "Continue";
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void WaveContinue()
    {
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }
}
