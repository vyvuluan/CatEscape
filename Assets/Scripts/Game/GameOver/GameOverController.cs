using Extensions;
using Parameters;
using Services;
using System.Collections;
using UnityEngine;
using Utilities;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameOverView view;
    private PopUpParameter popUpParameter;
    private GameServices gameServices;
    private AdsService adsService;
    private int colorIndex;
    private void Awake()
    {
        popUpParameter = PopupHelpers.PassParamPopup();
        GameObject gameServiecObject = GameObject.FindGameObjectWithTag(Constanst.ServicesTag);
        gameServices = gameServiecObject.GetComponent<GameServices>();
        adsService = gameServices.GetService<AdsService>();
        view.HideBtnContinue(adsService.IsRewardedReady());
        adsService.OnRewardedAdsLoad = view.HideBtnContinue;
        colorIndex = 0;
        view.ThrowIfNull();
    }
    private void Start()
    {
        float mapPercent = popUpParameter.GetObject<float>(Constanst.MapPercentKey);
        view.SetValueMapCurrent(mapPercent);
        StartCoroutine(ChangeCircleSizeAndColor(5f));
    }

    public void Close()
    {
        popUpParameter.GetAction(ActionType.CloseGameOver)?.Invoke();
    }
    private IEnumerator ChangeCircleSizeAndColor(float time)
    {
        view.SetGameOver(true);
        yield return new WaitForSeconds(1f);
        view.SetGameOver(false);
        int count = (int)time;
        while (count > 0)
        {
            float t = 0f;
            view.SetText(count.ToString());
            view.SetColorCircle(colorIndex);
            while (t < 1f)
            {
                t += Time.deltaTime;
                float scale = Mathf.Lerp(0.5f, 1.0f, t);
                view.CircleImage.rectTransform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            count--;
            colorIndex++;
        }
        popUpParameter.GetAction(ActionType.CloseGameOver)?.Invoke();
    }
    public void Continue()
    {
        popUpParameter.GetAction(ActionType.ContinueGameOver)?.Invoke();
    }

}
