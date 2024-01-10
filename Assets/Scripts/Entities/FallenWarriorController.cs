using UnityEngine;

public class FallenWarriorController : MonoBehaviour
{
    public int currency;
    [Range(0f, 1f)]
    [SerializeField] private float baseReturnPercentage;

    private Animator anim;
    private SpriteRenderer sr;

    private bool isPickedUp;
    [SerializeField] private float fallingSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isPickedUp)
            transform.position += new Vector3(0, -fallingSpeed * Time.deltaTime, 0);

        if (sr.color.a <= 0)
            Destroy(gameObject);
    }

    // idle animation would be cool, but no sprites for it ^@^
    public void TriggerDisappearAnimation() => anim.SetTrigger("Disappear");

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null) 
            return;

        float returnPercentage = baseReturnPercentage += Random.Range(-0.1f, 0.1f);

        if (returnPercentage > 0f)
            PlayerManager.instance.currency += (int)(currency * returnPercentage);

        // TODO: some fx as well?
        TriggerDisappearAnimation();
        isPickedUp = true;
    }
}
