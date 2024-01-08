using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject thunderStrikePrefab;

    public string Id;

    [SerializeField] private bool isActivated;
    public bool IsActivated
    {
        get 
        { 
            anim.SetBool("Active", isActivated); 
            return isActivated; 
        }
        set 
        {
            if (value && !isActivated)
                Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);

            anim.SetBool("Active", value);
            //Debug.Log("Setting IsActivated to: " +  value);
            isActivated = value;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint ID")]
    private void GenerateID()
    {
        if (string.IsNullOrEmpty(Id))
            Id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Player>())
            return;

        IsActivated = true;
    }
}
