using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // I have failed in adjusting this script to freely move the player on a scene

    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject player;
    [SerializeField] private float parallaxEffect;
    private float xPosition;
    private float length;

    void Start()
    {
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = 0;
    }

    void Update()
    {
        //transform.parent.position = new Vector2(
        //    transform.parent.position.x, 6.5f); // TODO: magical number

        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if (distanceMoved > xPosition + length)
        {
            xPosition += length;
        }
        else if (distanceMoved < xPosition - length )
        {
            xPosition -= length;
        }
    }
}