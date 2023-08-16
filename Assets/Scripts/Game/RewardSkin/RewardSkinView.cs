using Extensions;
using TMPro;
using UnityEngine;
public class RewardSkinView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idSkinInGetSkinText;
    [SerializeField] private GameObject btnGetSkin;
    private void Awake()
    {
        idSkinInGetSkinText.ThrowIfNull();
        btnGetSkin.ThrowIfNull();
    }
    public void SetIdSkinInGetSkinPanelText(int id)
    {
        idSkinInGetSkinText.text = id.ToString();
    }
    public void HideBtnGetSkin(bool status)
    {
        if(btnGetSkin != null)
            btnGetSkin.SetActive(status);
    }    
}
