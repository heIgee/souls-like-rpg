using UnityEngine;

public class FadeScreenUI : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void FadeIn() => anim.SetTrigger("fadeIn");
    public void FadeInDelay(float delay) => Invoke(nameof(FadeIn), delay);
    public void FadeOut() => anim.SetTrigger("fadeOut");


}
