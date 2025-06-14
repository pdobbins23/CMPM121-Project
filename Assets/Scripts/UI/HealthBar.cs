using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    public static int ColorBlindMode = 0;
    private static readonly (Color32 healthy, Color32 hurt)[] colorPairs = new[]
    {
        (new Color32(0, 192, 56, 255), new Color32(255, 8, 0, 255)),
        (new Color32(0, 107, 189, 255), new Color32(255, 251, 0, 255)),
        (new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255))
    };

    public GameObject background;
    public GameObject slider;

    public Hittable hp;
    float old_perc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var (healthy, hurt) = colorPairs[ColorBlindMode];

        background.GetComponent<SpriteRenderer>().color = hurt;
        slider.GetComponent<SpriteRenderer>().color = healthy;

        if (hp == null) return;
        float perc = hp.hp * 1.0f / hp.max_hp;
        if (Mathf.Abs(old_perc - perc) > 0.01f)
        {
            slider.transform.localScale = new Vector3(perc, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - perc) / 2, 0, 0);
            old_perc = perc;
        }
    }

    public void SetHealth(Hittable hp)
    {
        this.hp = hp;
        float perc = hp.hp * 1.0f / hp.max_hp;

        slider.transform.localScale = new Vector3(perc, 1, 1);
        slider.transform.localPosition = new Vector3(-(1-perc)/2, 0, 0);
        old_perc = perc;
    }
}
