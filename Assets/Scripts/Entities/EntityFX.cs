using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash effects")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Material hitMat;
    private Material origMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColors;
    [SerializeField] private Color[] igniteColors;
    [SerializeField] private Color[] shockColors;

    private bool isBlinking = false;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        origMat = sr.material;
    }

    public IEnumerator FlashFXCoroutine()
    {
        if (sr.material == hitMat)
            yield break;

        sr.material = hitMat;

        // will not work if applied ailment color is not an array
        // applies white then
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = origMat;
    }

    public void StartRedWhiteBlink()
    {
        isBlinking = true;
        StartCoroutine(RedWhiteBlinkCoroutine());
    }
    private IEnumerator RedWhiteBlinkCoroutine()
    {
        while (isBlinking)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RunIgniteFXFor(float seconds)
    {
        Invoke(nameof(CancelFX), seconds);
        InvokeRepeating(nameof(IgniteColorFX), 0, 0.3f);
    }


    private void IgniteColorFX()
    {
        if (sr.color != igniteColors[0])
            sr.color = igniteColors[0];
        else
            sr.color = igniteColors[1];
    }


    public void RunChillFXFor(float seconds)
    {
        Invoke(nameof(CancelFX), seconds);
        InvokeRepeating(nameof(ChillColorFX), 0, 0.3f);
    }

    private void ChillColorFX()
    {
        if (sr.color != chillColors[0])
            sr.color = chillColors[0];
        else
            sr.color = chillColors[1];
    }

    public void RunShockFXFor(float seconds)
    {
        Invoke(nameof(CancelFX), seconds);
        InvokeRepeating(nameof(ShockColorFX), 0, 0.3f);
    }

    private void ShockColorFX()
    {
        if (sr.color != shockColors[0])
            sr.color = shockColors[0];
        else
            sr.color = shockColors[1];
    }

    public void CancelFX()
    {
        isBlinking = false;

        // idk just killing everything
        StopAllCoroutines();
        CancelInvoke();

        sr.color = Color.white;
    }

    public void SetTransparency(bool bull) => sr.color = bull ? Color.clear : Color.white;
}
