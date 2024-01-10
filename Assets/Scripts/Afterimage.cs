using UnityEngine;

public class Afterimage : MonoBehaviour
{
    private SpriteRenderer sr;
    private float alphaLooseRate;

    public void SetupAfterimage(float alphaLooseRate, Sprite sprite)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = sprite;
        this.alphaLooseRate = alphaLooseRate;
    }

    private void Update()
    {
        if (alphaLooseRate <= 0)
            return;

        float alpha = sr.color.a - alphaLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (sr.color.a <= 0)
            Destroy(gameObject);
    }
}
