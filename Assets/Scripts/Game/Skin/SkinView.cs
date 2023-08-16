using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkinTab
{
    [SerializeField] private GameObject tab;
    [SerializeField] private GameObject btnClick;
    [SerializeField] private Image btnSelected;

    public GameObject Tab { get => tab; set => tab = value; }
    public GameObject BtnClick { get => btnClick; set => btnClick = value; }
    public Image BtnSelected { get => btnSelected; set => btnSelected = value; }
}

public class SkinView : MonoBehaviour
{
    #region SKIN
    [Header("SKIN")]
    [Space(8.0f)]
    [SerializeField] private Canvas main;
    [SerializeField] private RectTransform btnBottom;
    [SerializeField] private RectTransform backGround;
    [SerializeField] private GameObject leftNormal;
    [SerializeField] private GameObject leftSelected;
    [SerializeField] private GameObject rightNormal;
    [SerializeField] private GameObject rightSelected;
    [SerializeField] private GameObject imageBGEnemy;
    [SerializeField] private GameObject equippedGrayBtn;
    [SerializeField] private List<GameObject> listImageNewInBtn;
    [SerializeField] private List<GameObject> listImageNewInImage;
    [SerializeField] private List<SkinTab> listSkinBtns;
    [SerializeField] private List<RectTransform> parentSkinTab;
    public List<RectTransform> ParentSkinTab { get => parentSkinTab; set => parentSkinTab = value; }

    #endregion
    private void Awake()
    {
        main.ThrowIfNull();
        btnBottom.ThrowIfNull();
        leftNormal.ThrowIfNull();
        leftSelected.ThrowIfNull();
        rightNormal.ThrowIfNull();
        rightSelected.ThrowIfNull();
        equippedGrayBtn.ThrowIfNull();
    }
    public void SetActiveEquippedGrayBtn(bool status)
    {
        equippedGrayBtn.SetActive(status);
    }
    public void SetActiveImageNew(int index, bool status)
    {
        listImageNewInBtn[index].SetActive(status);
        listImageNewInImage[index].SetActive(status);
    }
    public void AvoidBanner(float bannerHeight)
    {
        bannerHeight /= main.scaleFactor;

        backGround.sizeDelta = new Vector2(backGround.sizeDelta.x, backGround.sizeDelta.y + bannerHeight + 237f);
        btnBottom.sizeDelta = new Vector2(btnBottom.sizeDelta.x, btnBottom.sizeDelta.y + bannerHeight + 237f);
    }
    public void SetTabOn(int index)
    {
        switch(index)
        {
            case 0:
            case 1:
                leftNormal.SetActive(false);
                rightNormal.SetActive(true);
                leftSelected.SetActive(true);
                rightSelected.SetActive(false);
                imageBGEnemy.SetActive(false);
                break;
            case 2:
            case 3:
                leftNormal.SetActive(true);
                rightNormal.SetActive(false);
                leftSelected.SetActive(false);
                rightSelected.SetActive(true);
                imageBGEnemy.SetActive(true);
                break;
        }
        for (int i = 0; i < listSkinBtns.Count; i++)
        {
            if (i == index)
            {
                listSkinBtns[i].Tab.SetActive(true);
                listSkinBtns[i].BtnClick.SetActive(false);
                listSkinBtns[i].BtnSelected.gameObject.SetActive(true);
            }
            else
            {
                listSkinBtns[i].Tab.SetActive(false);
                listSkinBtns[i].BtnClick.SetActive(true);
                listSkinBtns[i].BtnSelected.gameObject.SetActive(false);
            }
        }
    }

}
