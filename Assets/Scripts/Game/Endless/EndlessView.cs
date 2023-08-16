using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndlessView : MonoBehaviour
{
    [SerializeField] private GameObject bottomBtn;
    [SerializeField] private GameObject imageSkillBehind;
    [SerializeField] private GameObject pipeBackGround;
    [SerializeField] private GameObject groundBackGround;
    [SerializeField] private GameObject reportPanel;
    [SerializeField] private TextMeshProUGUI countDownShootText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI coinRewardText;
    [SerializeField] private Canvas main;
    [SerializeField] private Image loadImage;
    [SerializeField] private RectTransform bottomBtnRect;

    private void Awake()
    {
        bottomBtnRect.ThrowIfNull();
        bottomBtn.ThrowIfNull();
        main.ThrowIfNull();
        imageSkillBehind.ThrowIfNull();
        countDownShootText.ThrowIfNull();
        coinText.ThrowIfNull();
        reportPanel.ThrowIfNull();
        highScoreText.ThrowIfNull();
        currentScoreText.ThrowIfNull();
        coinRewardText.ThrowIfNull();
    }
    public void SetActivePanelReport(bool status)
    {
        reportPanel.SetActive(status);
    }
    public void SetParameterPanelReport(int highScore, int currentScore, int coinReward)
    {
        highScoreText.text = highScore.ToString();
        currentScoreText.text = currentScore.ToString();
        coinRewardText.text = coinReward.ToString();
    }
    public void SetCoin(int coin)
    {
        coinText.text = coin.ToString();
    }
    public void AvoidBanner(float bannerHeight)
    {
        bannerHeight /= main.scaleFactor;
        bottomBtnRect.sizeDelta = new Vector2(bottomBtnRect.sizeDelta.x, bottomBtnRect.sizeDelta.y + bannerHeight);
    }
    public void SetImageSkill(bool status)
    {
        imageSkillBehind.SetActive(status);
    }
    public void SetStatusListButton(bool status)
    {
        bottomBtn.SetActive(status);
    }
    public void SetOpacityImageLoading(float number)
    {
        loadImage.color = new Color(0, 0, 0, number);
    }
    public void SetGroundBackGround(bool status)
    {
        pipeBackGround.SetActive(!status);
        groundBackGround.SetActive(status);
    }
    public void TextCountDownShootEnemyTeacher(bool status, string text)
    {
        if (status)
        {
            countDownShootText.text = text;
        }

        countDownShootText.gameObject.SetActive(status);
    }
}
