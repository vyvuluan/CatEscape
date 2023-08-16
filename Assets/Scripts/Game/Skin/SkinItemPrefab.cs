using Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinItemPrefab : MonoBehaviour
{
    [SerializeField] private GameObject lockSkin;
    [SerializeField] private GameObject btnAds;
    [SerializeField] private GameObject btnEquip;
    [SerializeField] private GameObject btnEquiped;
    [SerializeField] private GameObject imageNew;
    [SerializeField] private GameObject imageSelected;
    [SerializeField] private Button buttonAds;
    [SerializeField] private Image skinImage;
    

    private SkinItem skinItem;
    private Action<SkinItem> onClick;

    public GameObject ImageNew { get => imageNew; }

    private void Awake()
    {
        lockSkin.ThrowIfNull();
        btnAds.ThrowIfNull();
        btnEquip.ThrowIfNull();
        buttonAds.ThrowIfNull();
        imageNew.ThrowIfNull();
        btnEquiped.ThrowIfNull();
        imageSelected.ThrowIfNull();
        skinImage.ThrowIfNull();
    }
    public void Init(RectTransform parent, SkinItem skinItem, Action<SkinItem> onClick)
    {
        transform.SetParent(parent, false);
        transform.localScale = Vector3.one;
        this.skinItem = skinItem;
        this.onClick = onClick;
        skinImage.sprite = skinItem.Image;
        switch (skinItem.State)
        {
            case StatusState.Own:
                SetOwn();
                SetSelected(false);
                break;
            case StatusState.Ads:
                SetAds();
                SetSelected(false);
                break;
            case StatusState.Lock:
                SetLock();
                SetSelected(false);
                break;
            case StatusState.Equip:
                SetEquip();
                SetSelected(true);
                break;
        }
        if (skinItem.IsSeen)
        {
            imageNew.SetActive(false);
        }
        else
        {
            if(skinItem.State != StatusState.Lock)
                imageNew.SetActive(true);
        }
    }
    public void OnClickButon()
    {
        onClick?.Invoke(skinItem);
    }    
    public void SetSelected(bool status)
    {
        imageSelected.SetActive(status);
    }    
    public void HideAdsSkin(bool status)
    {
        buttonAds.enabled = status;
    }    
    public void SetOwn()
    {
        skinItem.State = StatusState.Own;
        lockSkin.SetActive(false);
        btnAds.SetActive(false);
        btnEquip.SetActive(true);
        btnEquiped.SetActive(false);
    }
    public void SetEquip()
    {
        skinItem.State = StatusState.Equip;
        lockSkin.SetActive(false);
        btnAds.SetActive(false);
        btnEquip.SetActive(false);
        btnEquiped.SetActive(true);
    }
    public void SetAds()
    {
        skinItem.State = StatusState.Ads;
        lockSkin.SetActive(false);
        btnAds.SetActive(true);
        btnEquip.SetActive(false);
        btnEquiped.SetActive(false);
    }
    public void SetLock()
    {
        skinItem.State = StatusState.Lock;
        lockSkin.SetActive(true);
        btnAds.SetActive(false);
        btnEquip.SetActive(false);
        btnEquiped.SetActive(false);
    }
    public void SetActiveImageNew(bool status)
    {
        imageNew.SetActive(status);
    }
}
