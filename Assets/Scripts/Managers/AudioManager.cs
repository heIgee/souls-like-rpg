using System.Collections;
using UnityEngine;

//public enum SFX
//{
//    Footsteps = 14
//};

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBgm;
    private int bgmIndex;

    // TODO: enum for sfx and bgm instead of explicit indexes (easier to edit)

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    private void Update()
    {
        if (!playBgm)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    public void PlaySFX(int index, Transform source = null)
    {
        //if (sfx[index].isPlaying)
        //    return;

        if (index >= sfx.Length)
            return;

        if (source != null 
            && Vector2.Distance(source.position, PlayerManager.instance.transform.position)
            > sfxMinDistance)
            return;

        Debug.Log("Playing: " + sfx[index].name);

        if (index == 2)
            sfx[index].pitch = Random.Range(0.8f, 1.2f);

        sfx[index].Play();
    }

    public void StopSFX(int index)
    {
        if (index < sfx.Length)
            sfx[index].Stop();
    }
    
    public void StopSFXSlowly(int index)
    {
        if (index < sfx.Length)
            StartCoroutine(DecreaseVolumeCoroutine(sfx[index]));
    }

    public void PlayBGM(int index)
    {
        bgmIndex = index;

        StopAllBGM();

        playBgm = true;
        bgm[index].Play();
        Debug.Log("Playing: " + bgm[index].name);
    }

    public void StopAllBGM()
    {
        foreach (var bg in bgm)
            bg.Stop();
    }

    public void StopAllBGMSlowly()
    {
        foreach (var bg in bgm)
            StartCoroutine(DecreaseVolumeCoroutine(bg));
    }

    private IEnumerator DecreaseVolumeCoroutine(AudioSource audio)
    {
        float defaultVolume = audio.volume;

        while (audio.volume > 0.1f)
        {
            audio.volume *= 0.9f;
            yield return new WaitForSeconds(0.1f);
        }

        if (audio.volume < 0.1f)
        {
            audio.Stop();
            audio.volume = defaultVolume;
        }
    }

}
