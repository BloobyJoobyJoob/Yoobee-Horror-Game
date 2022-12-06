using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Image image;

    private float scale;
    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        scale = transform.localScale.x;

        button.onClick.AddListener(() =>
        {
            image.DOColor(MenuManager.Singleton.ButtonPressColor, MenuManager.Singleton.ButtonPressTime).SetEase(Ease.OutSine).OnComplete(() 
                => image.DOColor(MenuManager.Singleton.ButtonNormalColor, MenuManager.Singleton.ButtonPressTime).SetEase(Ease.OutSine));

            image.transform.DOScale(MenuManager.Singleton.ButtonPressScale * scale, MenuManager.Singleton.ButtonPressTime).SetEase(Ease.OutSine).OnComplete(() 
                => image.transform.DOScale(scale, MenuManager.Singleton.ButtonPressTime).SetEase(Ease.OutSine));
        });
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        image.DOColor(MenuManager.Singleton.ButtonHoverColor, MenuManager.Singleton.ButtonHoverTime).SetEase(Ease.OutSine);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        image.DOColor(MenuManager.Singleton.ButtonNormalColor, MenuManager.Singleton.ButtonHoverTime).SetEase(Ease.OutSine);
    }
}