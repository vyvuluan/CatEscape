using Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private GameObject btnContinue;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject revivalPanel;
    [SerializeField] private Image circleImage;
    [SerializeField] private Image inCircleImage;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Slider mapPrecent;
    [SerializeField] private List<Color> colors;

    public Image CircleImage { get => circleImage; }

    private void Awake()
    {
        mapPrecent.ThrowIfNull();
        btnContinue.ThrowIfNull();
        gameOverText.ThrowIfNull();
        revivalPanel.ThrowIfNull();
        circleImage.ThrowIfNull();
        inCircleImage.ThrowIfNull();
        textMeshProUGUI.ThrowIfNull();
        mapPrecent.ThrowIfNull();
    }
    public void SetValueMapCurrent(float value)
    {
        mapPrecent.value = value;
    }    
    public void SetText(string text)
    {
        textMeshProUGUI.text = text;
    }    
    public void SetColorCircle(int index)
    {
        circleImage.color = colors[index];
        inCircleImage.color = colors[index];
    }    
    public void HideBtnContinue(bool status)
    {
        if(btnContinue != null)
        {
            btnContinue.SetActive(status);
        }
    }
    public void SetGameOver(bool status)
    {
        revivalPanel.SetActive(!status);
        gameOverText.SetActive(status);
    }    
}
