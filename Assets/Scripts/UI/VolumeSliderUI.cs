using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    public Slider slider;
    public string parameter;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier;

    public void SetSliderValue(float value)
    {
        if (value <= 0.02f)
            audioMixer.SetFloat(parameter, -10000f);
        else
            audioMixer.SetFloat(parameter, Mathf.Log10(value) * multiplier);
    }

    public void LoadSLider(float value) => slider.value = Mathf.Clamp01(value);
}
