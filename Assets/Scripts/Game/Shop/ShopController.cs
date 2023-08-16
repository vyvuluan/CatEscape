using DG.Tweening;
using Extensions;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopView shopView;
    [SerializeField] private ShopModel shopModel;
    [SerializeField] private GameObject coinPrefab;
    private int coinCurrent;
    private int priceCurrent;
    private float elapsedTime;
    private GameServices gameServices;
    private PlayerService playerService;
    private AdsService adsService;
    private DisplayService displayService;
    private IAPService iapService;
    private AudioService audioService;
    private List<SkinItem> skinItems;
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
        displayService = gameServices.GetService<DisplayService>();
        iapService = gameServices.GetService<IAPService>();
        audioService = gameServices.GetService<AudioService>();
        elapsedTime = 0;
        coinCurrent = playerService.GetScore();
        priceCurrent = playerService.GetPriceSkin();
        InitSkinItems();
        shopView.HideBtnGetCoin(adsService.IsRewardedReady());
        adsService.OnRewardedAdsLoad = shopView.HideBtnGetCoin;

        shopView.SetTextCoin(coinCurrent);
        shopView.SetPriceSkin(priceCurrent);
        shopView.ThrowIfNull();
    }
    private void Start()
    {
        if (playerService.GetLevel() >= 2) shopView.AvoidBanner(adsService.GetHightBanner());
    }
    private void Update()
    {
        CheckAdsThreeMin();
    }
    public void CloseShop()
    {
        playerService.SetInterstitialAds(1);
        SceneManager.LoadScene(Constanst.MainScene);
    }

    public void PurchaseRandomSkin()
    {
        if (skinItems.Count <= 0) return;
        int skinIndex = Random.Range(0, skinItems.Count);
        List<int> skinOwned = playerService.GetSkinOwned();
        if (coinCurrent >= priceCurrent)
        {
            coinCurrent -= priceCurrent;
            if (priceCurrent == 50 || priceCurrent == 100 || priceCurrent == 150)
            {
                priceCurrent += 50;
            }
            else priceCurrent += 100;
            skinOwned.Add(skinItems[skinIndex].Id);
            playerService.SetSkinOwned(skinOwned);
            playerService.SetScore(coinCurrent);
            playerService.SetPriceSkin(priceCurrent);
            //update price and coin
            shopView.SetTextCoin(coinCurrent);
            shopView.SetPriceSkin(priceCurrent);
            //show buy success
            StartCoroutine(ShowPurchaseSusscess(skinItems[skinIndex].Id));
            skinItems.Remove(skinItems[skinIndex]);
        }
    }

    public void InitSkinItems()
    {
        //skinItem contain skin have lvlUnlock = -1 and not bought yet
        List<int> skinOwned = playerService.GetSkinOwned();
        skinItems = shopModel.SkinItems.Select(n => new SkinItem(n.Id, n.State, n.SkinType, n.Image, n.LvlUnlock, n.IsSeen)).Where(n => n.LvlUnlock == -1).ToList();
        //remove skin equip and Owned
        for (int i = skinItems.Count - 1; i >= 0; i--)
        {
            if (skinOwned.Contains(skinItems[i].Id))
            {
                skinItems.Remove(skinItems[i]);
            }
        }
    }
    private IEnumerator ShowPurchaseSusscess(int id)
    {
        shopView.SetOnSkinObject(true);
        shopView.SetSkinIdText(id);
        yield return new WaitForSeconds(1.5f);
        shopView.SetOnSkinObject(false);
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
            if (elapsedTime >= shopModel.InactivityThreshold)
            {
                InterstitialAds();
                elapsedTime = 0f;
            }
        }
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
    public void GetCoin()
    {
        adsService.InitRewardedAd(() =>
        {
            int coinPlus = 300;
            int currentCoin = playerService.GetScore();
            StartCoroutine(SpawnCoin(coinPlus, currentCoin));
            playerService.SetScore(currentCoin + coinPlus);
        }, () =>
        {
            Debug.Log("fail");
        });
        adsService.ShowRewardedAd();
    }
    private IEnumerator IncrementCoin(int coin, int totalCoin)
    {
        float increaseTime = 2f;
        int targetCoin = totalCoin + coin;
        float elapsedTime = 0.0f;
        while (totalCoin < targetCoin)
        {
            elapsedTime += Time.deltaTime;
            float incrementalValue = Mathf.Lerp(0, targetCoin - totalCoin, elapsedTime / increaseTime);
            totalCoin += Mathf.FloorToInt(incrementalValue);
            shopView.SetTextCoin(totalCoin);
            yield return null;
        }
    }
    private IEnumerator SpawnCoin(int coinPlus, int coinCur)
    {
        StartCoroutine(IncrementCoin(coinPlus, coinCur));
        for (int i = 0; i < 50; i++)
        {
            Vector3 temp = new Vector3(Random.Range(-4f, 4f), Random.Range(-12f, -11f), 0);
            GameObject go = SimplePool.Spawn(coinPrefab, temp, Quaternion.identity);
            go.transform.DOMoveY(go.transform.position.y + 15f, 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                go.transform.DOMoveY(go.transform.position.y - 3f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    SimplePool.Despawn(go);
                });
            });
            yield return new WaitForSeconds(0.05f);
        }

    }
}
