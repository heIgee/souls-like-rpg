using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;

    [Header("Popup text")]
    [SerializeField] protected GameObject popupTextPrefab;

    [Header("Flash effects")]
    [SerializeField] protected float flashDuration = 0.2f;
    [SerializeField] protected Material hitMat;
    protected Material origMat;

    [Header("Ailment colors")]
    [SerializeField] protected Color[] chillColors;
    [SerializeField] protected Color[] igniteColors;
    [SerializeField] protected Color[] shockColors;

    [Header("Ailment particles")]
    [SerializeField] protected ParticleSystem igniteFX;
    [SerializeField] protected ParticleSystem chillFX;
    [SerializeField] protected ParticleSystem shockFX;

    [Header("Hit FX")]
    [SerializeField] protected GameObject hitFXPrefab;
    [SerializeField] protected GameObject critHitFXPrefab;

    [Space]
    [SerializeField] protected ParticleSystem dustFX;

    protected bool isBlinking = false;

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        origMat = sr.material;
    }

    protected virtual void Update()
    {
        
    }

    public void CreatePopupText(string message)
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(2f, 3f);
        var offset = new Vector3(x, y, 0f);

        GameObject popup = Instantiate(popupTextPrefab, transform.position + offset, Quaternion.identity);
        popup.GetComponent<TextMeshPro>().text = message;
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
    protected IEnumerator RedWhiteBlinkCoroutine()
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
        igniteFX.Play();

        Invoke(nameof(CancelFX), seconds);
        InvokeRepeating(nameof(IgniteColorFX), 0, 0.3f);
    }

    protected void IgniteColorFX()
    {
        if (sr.color != igniteColors[0])
            sr.color = igniteColors[0];
        else
            sr.color = igniteColors[1];
    }

    public void RunChillFXFor(float seconds)
    {
        chillFX.Play();

        Invoke(nameof(CancelFX), seconds);
        InvokeRepeating(nameof(ChillColorFX), 0, 0.3f);
    }

    protected void ChillColorFX()
    {
        if (sr.color != chillColors[0])
            sr.color = chillColors[0];
        else
            sr.color = chillColors[1];
    }

    public void RunShockFXFor(float seconds)
    {
        shockFX.Play();

        Invoke(nameof(CancelFX), seconds);
        InvokeRepeating(nameof(ShockColorFX), 0, 0.3f);
    }

    protected void ShockColorFX()
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

        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();    
    }

    public void SetTransparency(bool bull) => sr.color = bull ? Color.clear : Color.white;

    public void CreateHitFX(Transform target, bool critical = false)
    {
        float zRotation = Random.Range(-90, 90);
        float xOffset = Random.Range(-0.2f, 0.2f);
        float yOffset = Random.Range(-0.5f, 0.5f);

        GameObject hitPrefab = critical ? critHitFXPrefab : hitFXPrefab;

        var position = new Vector2(target.transform.position.x + xOffset,
            target.transform.position.y + yOffset);

        GameObject hitFx = Instantiate(hitPrefab, position, Quaternion.identity);

        if (critical)
        {
            var entity = target.GetComponent<Entity>();

            //// Bard wrote it, does not work
            //float angle = Mathf.Atan2(entity.knockbackVector.y, entity.knockbackVector.x);

            //if (entity.KnockbackDir != 1)
            //    angle += Mathf.PI;

            //hitFx.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            zRotation = Random.Range(-20, 20);
            float yRotation = entity.KnockbackDir == -1 ? 180 : 0;

            hitFx.transform.Rotate(0, yRotation, zRotation);

            // only direction
            //hitFx.transform.localScale = new Vector3(entity.KnockbackDir, 1, 1);
        }
        else
            hitFx.transform.Rotate(0, 0, zRotation);

        Destroy(hitFx, 1f);
    }

    public void PlayDustFX() => dustFX.Play();
}
