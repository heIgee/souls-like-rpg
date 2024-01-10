using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("Screenshake")]
    [SerializeField] private float shakeMultiplier;
    [SerializeField] private Vector3 shakePower;
    // add different shake vectors for different situtations if you want
    private CinemachineImpulseSource screenShake;

    [Header("Afterimage effect")]
    [SerializeField] private GameObject afterimagePrefab;
    [SerializeField] private float afterimageAlphaLooseRate;
    [SerializeField] private float afterimageCooldown;
    private float afterimageCooldownTimer;

    protected override void Start()
    {
        base.Start();

        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Update()
    {
        base.Update();

        afterimageCooldownTimer -= Time.deltaTime;
    }

    public void ShakeScreen()
    {
        screenShake.m_DefaultVelocity = new Vector3(
            shakePower.x * PlayerManager.instance.player.FacingDirection, shakePower.y) * shakeMultiplier;

        screenShake.GenerateImpulse();
    }

    public void CreateAfterimage()
    {
        if (afterimageCooldownTimer > 0)
            return;

        GameObject afterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);
        afterimage.GetComponent<Afterimage>().SetupAfterimage(afterimageAlphaLooseRate, sr.sprite);
        afterimageCooldownTimer = afterimageCooldown;
    }
}
