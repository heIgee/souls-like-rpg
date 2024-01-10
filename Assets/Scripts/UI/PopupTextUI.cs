using TMPro;
using UnityEngine;

public class PopupTextUI : MonoBehaviour
{
    private TextMeshPro tmp;

    [SerializeField] private float baseSpeed;
    //[SerializeField] private float disappearanceSpeed;
    [SerializeField] private float baseAlphaLoseRate;

    private float speed;
    private float alphaLoseRate;

    //private float textTimer;

    private void Start()
    {
        speed = baseSpeed;
        alphaLoseRate = baseAlphaLoseRate;

        tmp = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, 
            new Vector2(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);

        //if (tmp.color.a < 0.2)
        //    alphaLoseRate *= 2f;
            //speed = disappearanceSpeed;

        if (tmp.color.a <= 0)
            Destroy(gameObject, 1f);

        float alpha = tmp.color.a - alphaLoseRate * Time.deltaTime; 
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
    }

}
