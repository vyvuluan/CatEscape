using Extensions;
using Parameters;
using Services;
using UnityEngine;
using Utilities;
public class RewardSkinController : MonoBehaviour
{
    [SerializeField] private RewardSkinView view;
    private PopUpParameter popUpParameter;
    private GameServices gameServices;
    private AdsService adsService;
    private void Awake()
    {
        popUpParameter = PopupHelpers.PassParamPopup();
        GameObject gameServiecObject = GameObject.FindGameObjectWithTag(Constanst.ServicesTag);
        gameServices = gameServiecObject.GetComponent<GameServices>();
        adsService = gameServices.GetService<AdsService>();
        view.HideBtnGetSkin(adsService.IsRewardedReady());
        adsService.OnRewardedAdsLoad = view.HideBtnGetSkin;
        view.ThrowIfNull();
    }
    private void Start()
    {
        InfoSkin();
    }
    public void Close()
    {
        popUpParameter.GetAction(ActionType.NoThanksSkin)?.Invoke();
    }
    public void GetSkin()
    {
        popUpParameter.GetAction(ActionType.AdsRewardSkin)?.Invoke();
    }
    public void InfoSkin()
    {
        //Get skinItem
        SkinItem skinItem = popUpParameter.GetObject<SkinItem>(Constanst.SkinItemKey);
        view.SetIdSkinInGetSkinPanelText(skinItem.Id);
    }    
}
