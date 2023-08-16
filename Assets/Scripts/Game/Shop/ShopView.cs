using Extensions;
using TMPro;
using UnityEngine;
public class ShopView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI skinIdText;
    [SerializeField] private TextMeshProUGUI priceSkin;
    [SerializeField] private Canvas main;
    [SerializeField] private GameObject skinObject;
    [SerializeField] private GameObject btnBottom;
    [SerializeField] private GameObject btnGetCoin;
    [SerializeField] private RectTransform btnBottomRect;
    private void Awake()
    {
        coinText.ThrowIfNull();
        skinIdText.ThrowIfNull();
        skinObject.ThrowIfNull();
        priceSkin.ThrowIfNull();
        btnGetCoin.ThrowIfNull();
        btnBottom.ThrowIfNull();
        main.ThrowIfNull();
        btnBottomRect.ThrowIfNull();
    }
    public void SetTextCoin(int coin)
    {
        coinText.text = coin.ToString();
    }
    public void AvoidBanner(float bannerHeight)
    {
        bannerHeight /= main.scaleFactor;
        btnBottomRect.sizeDelta = new Vector2(btnBottomRect.sizeDelta.x, btnBottomRect.sizeDelta.y + bannerHeight);
    }
    public void SetSkinIdText(int id)
    {
        skinIdText.text = id.ToString();
    }
    public void SetOnSkinObject(bool status)
    {
        skinObject.SetActive(status);
    }
    public void SetPriceSkin(int price)
    {
        priceSkin.text = price.ToString();
    }
    public void HideBtnGetCoin(bool status)
    {
        if (btnGetCoin != null)
        {
            btnGetCoin.SetActive(status);
        }
    }

}
