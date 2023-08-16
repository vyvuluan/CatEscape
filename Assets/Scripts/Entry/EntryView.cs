using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntryView : MonoBehaviour
{
    [SerializeField] private Slider loadingSilder;
    [SerializeField] private TextMeshProUGUI precent;
    private void Awake()
    {
        loadingSilder.ThrowIfNull();
        precent.ThrowIfNull();
    }
    public void SetValuePrecent(float value)
    {
        loadingSilder.value = value;
        precent.text = $"{value * 100}%";
    }
}
