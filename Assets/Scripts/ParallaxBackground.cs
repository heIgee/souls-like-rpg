using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float xPosition;
    private float length;

    void Start()
    {
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if(distanceMoved > xPosition + length)
        {
            xPosition += length; 
        }
        else if (distanceMoved < xPosition - length)
        {
            xPosition -= length;
        }
    }
}
