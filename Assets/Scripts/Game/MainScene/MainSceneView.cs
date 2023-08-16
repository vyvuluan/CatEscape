using DG.Tweening;
using Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneView : MonoBehaviour
{

    [SerializeField] private GameObject imageSkillBehind;
    [SerializeField] private GameObject panelFinishMap;
    [SerializeField] private GameObject pipeBackGround;
    [SerializeField] private GameObject groundBackGround;
    [SerializeField] private GameObject lvlUnlockSkin;
    [SerializeField] private GameObject bottomBtn;
    [SerializeField] private GameObject finishText;
    [SerializeField] private GameObject selectCharacter;
    [SerializeField] private GameObject btnRemoveAds;
    [SerializeField] private GameObject endlessModeLock;
    [SerializeField] private GameObject endlessModeUnlock;
    [SerializeField] private RectTransform plateCharacter;
    [SerializeField] private TextMeshProUGUI countDownShootText;
    [SerializeField] private TextMeshProUGUI percentUnlockText;
    [SerializeField] private TextMeshProUGUI idSkinText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI lvlCurrentText;
    [SerializeField] private Image percentLvlUnlock;
    [SerializeField] private Image loadImage;
    [SerializeField] private Image avatar;
    [SerializeField] private Image skinReward;
    [SerializeField] private RectTransform bottomBtnRect;
    [SerializeField] private Slider sliderMap;
    [SerializeField] private Slider sliderPercentSkinReward;
    [SerializeField] private Canvas main;
    [SerializeField] private List<GameObject> buttonLists;
    [SerializeField] private List<Button> btnGetCoin;

    private void Awake()
    {
        endlessModeLock.ThrowIfNull();
        imageSkillBehind.ThrowIfNull();
        panelFinishMap.ThrowIfNull();
        endlessModeUnlock.ThrowIfNull();
        pipeBackGround.ThrowIfNull();
        selectCharacter.ThrowIfNull();
        plateCharacter.ThrowIfNull();
        groundBackGround.ThrowIfNull();
        lvlCurrentText.ThrowIfNull();
        bottomBtn.ThrowIfNull();
        btnRemoveAds.ThrowIfNull();
        finishText.ThrowIfNull();
        countDownShootText.ThrowIfNull();
        idSkinText.ThrowIfNull();
        percentLvlUnlock.ThrowIfNull();
        coinText.ThrowIfNull();
        percentLvlUnlock.ThrowIfNull();
        avatar.ThrowIfNull();
        bottomBtnRect.ThrowIfNull();
        loadImage.ThrowIfNull();
        sliderMap.ThrowIfNull();
        sliderPercentSkinReward.ThrowIfNull();
        main.ThrowIfNull();
        skinReward.ThrowIfNull();
    }
    public void SetActiveLockEndless(bool status)
    {
        endlessModeLock.SetActive(status);
        endlessModeUnlock.SetActive(!status);
    }
    public void SelectCharacterHandle(float x)
    {
        plateCharacter.DOMoveX(x, 1f).OnComplete(() =>
        {
            selectCharacter.SetActive(false);

        });
    }
    public void SetActivePlateCharater(bool status)
    {
        plateCharacter.gameObject.SetActive(status);
    }
    public void SetParamaterWinPopup(float value, Sprite sprite)
    {
        sliderPercentSkinReward.value = value;
        //idSkinText.text = skinId.ToString();
        skinReward.sprite = sprite;
        percentUnlockText.text = $"{Mathf.CeilToInt(value * 100f)}%";
        percentLvlUnlock.fillAmount = value;
    }
    public void SetSilderValue(float value)
    {
        sliderMap.value = value;
    }

    public void SetActiveFinishText(bool status)
    {
        finishText.SetActive(status);
    }
    public void ShowRemoveAdsButton(bool status)
    {
        btnRemoveAds.SetActive(status);
    }
    public void OnPanelFinishMap(bool status)
    {
        panelFinishMap.SetActive(status);
    }
    public void SetImageSkill(bool status)
    {
        imageSkillBehind.SetActive(status);
    }
    public void TextCountDownShootEnemyTeacher(bool status, string text)
    {
        if (status)
        {
            countDownShootText.text = text;
        }

        countDownShootText.gameObject.SetActive(status);
    }
    public void SetGroundBackGround(bool status)
    {
        pipeBackGround.SetActive(!status);
        groundBackGround.SetActive(status);
    }
    public void SetLevelText(float lvlCurrent)
    {
        lvlCurrentText.text = lvlCurrent.ToString();

    }
    public void SetStatusListButton(bool status)
    {
        foreach (var btnList in buttonLists)
        {
            btnList.SetActive(status);
        }
    }
    public void SetWinPanel(float percent, int skinId)
    {
        idSkinText.text = skinId.ToString();
        percentUnlockText.text = $"{percent * 100f}%";
        percentLvlUnlock.fillAmount = percent;

    }
    public void SetLvlUnlockSkin(bool status)
    {
        lvlUnlockSkin.SetActive(status);
    }
    public void SetCoin(int coin)
    {
        coinText.text = coin.ToString();
    }
    public void SetStatusBtnGetCoin(bool status)
    {
        foreach (var btn in btnGetCoin)
        {
            btn.enabled = status;
        }
    }
    public void SetOpacityImageLoading(float number)
    {
        loadImage.color = new Color(0, 0, 0, number);
    }
    public void AvoidBanner(float bannerHeight)
    {
        bannerHeight /= main.scaleFactor;
        bottomBtnRect.sizeDelta = new Vector2(bottomBtnRect.sizeDelta.x, bottomBtnRect.sizeDelta.y + bannerHeight);
    }
    public void SetOrderLayoutCanvas(int order)
    {
        main.sortingOrder = order;
    }
}
