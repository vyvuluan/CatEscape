using Extensions;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinController : MonoBehaviour
{
    [SerializeField] private SkinModel skinModel;
    [SerializeField] private SkinView skinView;

    [SerializeField] private GameObject prefabItemSkin;
    private float lvlCurrent;
    private float elapsedTime;
    private GameServices gameServices;
    private PlayerService playerService;
    private AdsService adsService;
    private IAPService iapService;
    private List<int> skinOwned;
    private List<int> skinEquip;
    private List<int> skinSeen;
    private List<SkinItem> skinItems;
    private List<SkinItem> skinItemEquip;
    private List<SkinItem> skinItemSelected;
    private Dictionary<int, SkinItemPrefab> dicSkins;
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag(Constanst.ServicesTag) == null)
        {
            SceneManager.LoadScene(Constanst.EntryScene);
        }
        else
        {
            GameObject gameServiecObject = GameObject.FindGameObjectWithTag(Constanst.ServicesTag);
            gameServices = gameServiecObject.GetComponent<GameServices>();
        }
        playerService = gameServices.GetService<PlayerService>();
        adsService = gameServices.GetService<AdsService>();
        iapService = gameServices.GetService<IAPService>();
        skinOwned = playerService.GetSkinOwned();
        skinEquip = playerService.GetSkinEquip();
        skinSeen = playerService.GetSkinSeen();
        lvlCurrent = playerService.GetLevel();
        skinItemEquip = new List<SkinItem>(4) {null, null, null, null};
        skinItemSelected = new List<SkinItem>(4) {null, null, null, null};
        dicSkins = new();
        elapsedTime = 0;
        InitListSkinItem();
        InitSkin();

        prefabItemSkin.ThrowIfNull();
        skinView.ThrowIfNull();
        skinModel.ThrowIfNull();
    }
    private void Start()
    {
        if (playerService.GetLevel() >= 2) skinView.AvoidBanner(adsService.GetHightBanner());
    }
    private void Update()
    {
        CheckAdsThreeMin();
    }
    public void CloseSkin()
    {
        playerService.SetInterstitialAds(1);
        SceneManager.LoadScene(Constanst.MainScene);
    }
    
    public void InitListSkinItem()
    {
        //copy list skinItem
        skinItems = skinModel.SkinItems.Select(n => new SkinItem(n.Id, n.State, n.SkinType, n.Image, n.LvlUnlock, n.IsSeen)).ToList();
        foreach (var skin in skinItems)
        {
            if (skinOwned.Contains(skin.Id))
            {
                if (skinEquip.Contains(skin.Id))
                {
                    skin.State = StatusState.Equip;
                }
                else skin.State = StatusState.Own;
            }
            else if (skin.LvlUnlock <= lvlCurrent && skin.LvlUnlock != -1)
            {
                skin.State = StatusState.Ads;
            }
            else skin.State = StatusState.Lock;
            if (skinSeen.Contains(skin.Id))
            {
                skin.IsSeen = true;
            }
            
        }
        //Set image new for btn
        for (int index = 0; index < 4; index++)
        {
            bool isItemNew = CheckItemNew(index);
            skinView.SetActiveImageNew(index, isItemNew);
        }
    }
    public void InitSkin()
    {
        for (int i = 0; i < skinItems.Count; i++)
        {
            SkinItemPrefab prefab = SimplePool.Spawn(prefabItemSkin, Vector3.zero, Quaternion.identity).GetComponent<SkinItemPrefab>();
            switch (skinItems[i].SkinType)
            {
                case SkinType.Type1:
                    prefab.Init(skinView.ParentSkinTab[0], skinItems[i], EventSkinItem);
                    break;
                case SkinType.Type2:
                    prefab.Init(skinView.ParentSkinTab[1], skinItems[i], EventSkinItem);
                    break;
                case SkinType.Type3:
                    prefab.Init(skinView.ParentSkinTab[2], skinItems[i], EventSkinItem);
                    break;
                case SkinType.Type4:
                    prefab.Init(skinView.ParentSkinTab[3], skinItems[i], EventSkinItem);
                    break;
                default:
                    break;
            }
            if (skinItems[i].State == StatusState.Ads)
            {
                prefab.HideAdsSkin(adsService.IsRewardedReady());
                adsService.OnRewardedAdsLoad = prefab.HideAdsSkin;
            }
            //init list skin use and select
            if (skinItems[i].State == StatusState.Equip)
            {
                int skinItemTempIndex = (int)skinItems[i].SkinType;
                skinItemEquip[skinItemTempIndex] = skinItems[i];
                skinItemSelected[skinItemTempIndex] = skinItems[i];
            }
            dicSkins.Add(skinItems[i].Id, prefab);
        }
    }    
    public void EventSkinItem(SkinItem skinItem)
    {
        switch (skinItem.State)
        {
            case StatusState.Ads:
                adsService.InitRewardedAd(() =>
                {
                    dicSkins[skinItem.Id].SetOwn();
                    skinOwned.Add(skinItem.Id);
                    playerService.SetSkinOwned(skinOwned);
                    dicSkins[skinItem.Id].SetActiveImageNew(false);
                    skinSeen.Add(skinItem.Id);
                    playerService.SetSkinSeen(skinSeen);
                }, () =>
                {
                    Debug.Log("fail");
                });
                adsService.ShowRewardedAd();
                break;
            case StatusState.Own:
                //Select skin
                int skinItemTempIndex = (skinItem.SkinType == SkinType.Type1)? 0 : (skinItem.SkinType == SkinType.Type2)? 1
                    : (skinItem.SkinType == SkinType.Type3)? 2 : 3;
                if (skinItemSelected[skinItemTempIndex] != null)
                {
                    dicSkins[skinItemSelected[skinItemTempIndex].Id].SetSelected(false);
                }
                skinItemSelected[skinItemTempIndex] = skinItem;
                dicSkins[skinItemSelected[skinItemTempIndex].Id].SetSelected(true);
                skinView.SetActiveEquippedGrayBtn(true);
                //seen
                dicSkins[skinItem.Id].SetActiveImageNew(false);
                skinSeen.Add(skinItem.Id);
                playerService.SetSkinSeen(skinSeen);
                skinItem.IsSeen = true;
                if(!CheckItemNew(skinItemTempIndex))
                {
                    skinView.SetActiveImageNew(skinItemTempIndex, false);
                }    
                break;
        }
    }  
    //use for Equipped button
    public void EquippedButton()
    {
        skinEquip = new();
        foreach (var item in skinItemEquip)
        {
            dicSkins[item.Id].SetOwn();
        }
        skinItemEquip = new();
        foreach (var item in skinItemSelected)
        {
            dicSkins[item.Id].SetEquip();
            skinEquip.Add(item.Id);
            skinItemEquip.Add(item);
        }
        skinView.SetActiveEquippedGrayBtn(false);
        playerService.SetSkinEquip(skinEquip);
    }   
    public void BtnRamdomSkin(int index)
    {
        StartCoroutine(RandomSkin(index));
    }    
    private IEnumerator RandomSkin(int tabIndex)
    {

        List<SkinItem> listTemp = new();
        foreach (var item in skinItems)
        {
            if(item.SkinType == (SkinType)tabIndex && item.Id != skinItemSelected[tabIndex].Id && item.State == StatusState.Own)
            {
                listTemp.Add(item);
            }    
        }
        if (listTemp.Count <= 0) yield break;
        int indexRandom = UnityEngine.Random.Range(0, listTemp.Count);
        dicSkins[skinItemSelected[tabIndex].Id].SetSelected(false);
        for (int i = 0; i <= indexRandom; i++)
        {
            dicSkins[listTemp[i].Id].SetSelected(true);
            yield return new WaitForSeconds(0.2f);
            if(i != indexRandom)
                dicSkins[listTemp[i].Id].SetSelected(false);
        }
        skinView.SetActiveEquippedGrayBtn(true);
        skinItemSelected[tabIndex] = listTemp[indexRandom];

    }    
    public bool CheckItemNew(int index)
    {
        foreach (var item in skinItems)
        {
            if(!item.IsSeen && item.SkinType == (SkinType)index && item.State != StatusState.Lock)
            {
                return true;
            }    
        }
        return false;
    }
    public void InterstitialAds()
    {
        if (adsService.IsInterstitialReady() == true && iapService.IsRemoveAds() == false)
        {
            adsService.OnInterstitialClose = () =>
            {

            };
            adsService.ShowLimitInterstitialAd();
        }
        else
        {

        }
    }
    public void CheckAdsThreeMin()
    {
        if (Input.anyKeyDown)
        {
            elapsedTime = 0f;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= skinModel.InactivityThreshold)
            {
                InterstitialAds();
                elapsedTime = 0f;
            }
        }
    }
}
