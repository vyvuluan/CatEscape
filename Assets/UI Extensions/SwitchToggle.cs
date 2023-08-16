using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SwitchToggle : MonoBehaviour
{
	[SerializeField] RectTransform uiHandleRectTransform;
	[SerializeField] RectTransform uiHandleRectTransform1;
	[SerializeField] Color backgroundActiveColor;
	[SerializeField] Color handleActiveColor;

	Image backgroundImage, handleImage;

	Color backgroundDefaultColor, handleDefaultColor;

	Toggle toggle;

	Vector2 handlePosition;

	void Awake()
	{
		toggle = GetComponent<Toggle>();

		handlePosition = uiHandleRectTransform.anchoredPosition;

		backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
		handleImage = uiHandleRectTransform.GetComponent<Image>();

		backgroundDefaultColor = backgroundImage.color;
		handleDefaultColor = handleImage.color;

		toggle.onValueChanged.AddListener(OnSwitch);

		if (toggle.isOn)
			OnSwitch(true);
	}

	void OnSwitch(bool on)
	{
		//uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
		uiHandleRectTransform.DOAnchorPos(!on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);

        uiHandleRectTransform1.DOAnchorPos(!on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);

		if (!on)
		{
			uiHandleRectTransform.gameObject.SetActive(false);
			uiHandleRectTransform1.gameObject.SetActive(true);
		}
		else
		{
            uiHandleRectTransform.gameObject.SetActive(true);
            uiHandleRectTransform1.gameObject.SetActive(false);
        }	
		//backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor ; // no anim
		//backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .6f);

		//handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
		//handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, .4f);
	}


	void OnDestroy()
	{
		toggle.onValueChanged.RemoveListener(OnSwitch);
	}
}
