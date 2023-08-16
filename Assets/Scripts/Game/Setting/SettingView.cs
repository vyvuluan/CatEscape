using Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : MonoBehaviour
{
    [SerializeField] private Toggle vibrate;
    [SerializeField] private Slider sliderSound;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private List<GameObject> sounds;
    [SerializeField] private List<GameObject> mucsics;
    [SerializeField] private List<GameObject> vibrtes;
    private void Awake()
    {
        vibrate.ThrowIfNull();
        sliderSound.ThrowIfNull();
        sliderMusic.ThrowIfNull();
    }
    public void SetActiveImageMuteSound(bool status)
    {
        sounds[0].SetActive(!status);
        sounds[1].SetActive(status);
    }
    public void SetActiveImageMuteMusic(bool status)
    {
        mucsics[0].SetActive(!status);
        mucsics[1].SetActive(status);
    }
    public void SetActiveImageMuteVibrate(bool status)
    {
        vibrtes[0].SetActive(!status);
        vibrtes[1].SetActive(status);
    }
    public void SetVibrate(bool status)
    {
        vibrate.isOn = status;
    }
    public Toggle GetVibrate()
    {
        return vibrate;
    }
    public void SetSliderSound(float value)
    {
        sliderSound.value = value;
    }
    public void SetSliderMusic(float value)
    {
        sliderMusic.value = value;
    }
    public float GetValueSliderSound()
    {
        return sliderSound.value;
    }
    public float GetValueSliderMusic()
    {
        return sliderMusic.value;
    }

}
