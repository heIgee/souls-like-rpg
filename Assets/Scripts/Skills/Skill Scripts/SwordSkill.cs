using System;
using UnityEngine;

public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private float maxTravelDistance = 7f;
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float spinGravity = 1f;

    [Header("Aim dots")]
    [SerializeField] private int dotsNumber;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotsPrefab;
    [SerializeField] private Transform dotsParent;


    private SwordController controller;

    private GameObject[] dots;
    private Vector2 finalDir;

    public override void Use()
    {
        // using sword skill through ThrowSword
    }

    protected override void Start()
    {
        base.Start();

        GenerateDots();
        SetupGravity();
    }

    private void SetupGravity()
    {
        swordGravity = swordType switch
        {
            SwordType.Bounce => bounceGravity,
            SwordType.Pierce => pierceGravity,
            SwordType.Spin => spinGravity,
            _ => swordGravity
        };

    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x,
                AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
            for (int i = 0; i < dots.Length; i++) 
                dots[i].transform.position = CalculateDotPosition(i * spaceBetweenDots);
    }

    public void ThrowSword()
    {
        GameObject sword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        controller = sword.GetComponent<SwordController>();

        //(controller, swordType) switch
        //{
        //    (var c, SwordType.Bounce) => c.SetupBounce(true, bounceAmount),
        //    (var c, SwordType.Pierce) => c.SetupPierce(pierceAmount),
        //};

        //controller.Invoke(swordType switch
        //{
        //    SwordType.Bounce => controller.SetupBounce(true, bounceAmount),
        //    SwordType.Pierce => controller.SetupPierce(pierceAmount),
        //    _ => ""
        //}, 0);

        switch (swordType)
        {
            case SwordType.Bounce:
                controller.SetupBounce(true, bounceAmount, bounceSpeed);
                break;

            case SwordType.Pierce:
                controller.SetupPierce(pierceAmount);
                break;

            case SwordType.Spin:
                controller.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
                break;

            default:
                //
                break;
        }

        controller.SetupSword(swordType, finalDir, swordGravity, freezeTimeDuration, returnSpeed);

        player.ThrowSword(sword);

        SetDotsActive(false);
    }

    #region Aiming
    public Vector2 AimDirection()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos - playerPos;
    }

    public void SetDotsActive(bool isActive)
    {
        if (dots == null || dots.Length <= 0)
            return;

        foreach (var dot in dots)
            dot.SetActive(isActive);
    }

    private void GenerateDots()
    {
        dots = new GameObject[dotsNumber];

        for (int i = 0; i < dotsNumber; i++)
        {
            dots[i] = Instantiate(dotsPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }

        // CS1656 - fun fact, foreach vars are read-only
    }

    private Vector2 CalculateDotPosition(float t)
    {
        // kinematic equation 
        return (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x, 
            AimDirection().normalized.y * launchForce.y) * t 
            + t * t * (Physics2D.gravity * swordGravity) / 2f;
    }
    #endregion 
}
