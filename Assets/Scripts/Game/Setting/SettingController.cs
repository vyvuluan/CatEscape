using Extensions;
using Parameters;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
public class SettingController : MonoBehaviour
{
    [SerializeField] private SettingView view;
    private PopUpParameter popUpParameter;
    private GameServices gameServices;
    private PlayerService playerService;
    private void Awake()
    {
        popUpParameter = PopupHelpers.PassParamPopup();
        GameObject gameServiecObject = GameObject.FindGameObjectWithTag(Constanst.ServicesTag);
        gameServices = gameServiecObject.GetComponent<GameServices>();
        playerService = gameServices.GetService<PlayerService>();

        view.ThrowIfNull();
    }
    private void Start()
    {
        InitSetting();
    }
    public void InitSetting()
    {
        float music = playerService.GetMusicVolume();
        float sound = playerService.GetSoundVolume();
        bool vibrate = playerService.GetVibrate();
        if (music == 0)
        {
            view.SetActiveImageMuteMusic(true);
        }
        else view.SetActiveImageMuteMusic(false);
        if (sound == 0)
        {
            view.SetActiveImageMuteSound(true);
        }
        else view.SetActiveImageMuteSound(false);
        view.SetActiveImageMuteVibrate(!vibrate);
        view.SetVibrate(vibrate);
        view.SetSliderMusic(music);
        view.SetSliderSound(sound);
    }
    public void OnToggleVibrateValueChanged()
    {
        if (view.GetVibrate().isOn)
        {
            playerService.SetVibrate(true);
            view.SetActiveImageMuteVibrate(false);
        }
        else
        {
            playerService.SetVibrate(false);
            view.SetActiveImageMuteVibrate(true);
        }
    }
    public void ChangVibrate()
    {
        bool status = playerService.GetVibrate();
        if (status)
        {
            playerService.SetVibrate(false);
            view.SetActiveImageMuteVibrate(true);
            view.GetVibrate().isOn = false;
        }
        else
        {
            playerService.SetVibrate(true);
            view.SetActiveImageMuteVibrate(false);
            view.GetVibrate().isOn = true;
        }
    }
    public void EventPointerDownSliderSound()
    {
        float value = view.GetValueSliderSound();
        playerService.SetSoundVolume(value);
        if (value == 0)
        {
            view.SetActiveImageMuteSound(true);
        }
        else
        {
            view.SetActiveImageMuteSound(false);
        }
    }
    public void ChangeSound()
    {
        float value = view.GetValueSliderSound();
        if (value < 0.5f)
        {
            view.SetSliderSound(1);
            view.SetActiveImageMuteSound(false);
            playerService.SetSoundVolume(1);
        }
        else
        {
            view.SetSliderSound(0);
            view.SetActiveImageMuteSound(true);
            playerService.SetSoundVolume(0);
        }
    }
    public void ChangeMusic()
    {
        float value = view.GetValueSliderMusic();
        if (value < 0.5f)
        {
            view.SetSliderMusic(1);
            view.SetActiveImageMuteMusic(false);
            playerService.SetMusicVolume(1);
        }
        else
        {
            view.SetSliderMusic(0);
            view.SetActiveImageMuteMusic(true);
            playerService.SetMusicVolume(0);
        }
    }
    public void EventPointerDownSliderMusic()
    {
        float value = view.GetValueSliderMusic();
        playerService.SetMusicVolume(value);
        if (value == 0)
        {
            view.SetActiveImageMuteMusic(true);
        }
        else
        {
            view.SetActiveImageMuteMusic(false);
        }
    }
    public void Close()
    {
        popUpParameter.GetAction(ActionType.CloseSetting)?.Invoke();
    }
    public void ResetMap()
    {
        popUpParameter.GetAction(ActionType.ResetMap)?.Invoke();
    }
    public void Home()
    {
        SceneManager.LoadScene(Constanst.MainScene);
        Time.timeScale = 1f;
    }


}
