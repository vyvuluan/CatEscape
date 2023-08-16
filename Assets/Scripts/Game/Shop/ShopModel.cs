using System.Collections.Generic;
using UnityEngine;

public class ShopModel : MonoBehaviour
{
    #region SKIN
    [Header("SKIN")]
    [Space(8.0f)]
    [Tooltip("list skin")]
    [SerializeField] private List<SkinItem> skinItems;
    public List<SkinItem> SkinItems { get => skinItems;}
    #endregion

    #region ADS
    [Header("ADS")]
    [Space(8.0f)]
    [Tooltip("Time show ads if no tap screen")]
    [SerializeField] private float inactivityThreshold = 180f;
    public float InactivityThreshold { get => inactivityThreshold;}

    #endregion

}
