using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash effects")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Material hitMat;
    private Material origMat;

    private bool isBlinking = false;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        origMat = sr.material;
    }

    public IEnumerator FlashFX()
    {
        sr.material = hitMat;
        yield return new WaitForSeconds(flashDuration);
        sr.material = origMat;
    }

    public void StartRedWhiteBlink()
    {
        isBlinking = true;
        StartCoroutine(BlinkCoroutine());
    }
    private IEnumerator BlinkCoroutine()
    {
        while (isBlinking)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void CancelRedWhiteBlink()
    {
        isBlinking = false;
        sr.color = Color.white;
    }

}
