using UnityEngine;


public class AreaSound : MonoBehaviour
{
    private enum SoundType { SFX, BGM };

    [SerializeField] private SoundType soundType;
    [SerializeField] private int areaSoundIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null)
            return;

        switch (soundType)
        {
            case SoundType.SFX:
                AudioManager.instance.PlaySFX(areaSoundIndex); break;

            case SoundType.BGM: 
                AudioManager.instance.PlayBGM(areaSoundIndex); break;

            default: Debug.LogError($"{gameObject.name} - sound type missing"); break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null)
            return;

        switch (soundType)
        {
            case SoundType.SFX:
                AudioManager.instance.StopSFXSlowly(areaSoundIndex); break;

            case SoundType.BGM:
                AudioManager.instance.StopAllBGMSlowly(); break;

            default: Debug.LogError($"{gameObject.name} - sound type missing"); break;
        }
    }
}
