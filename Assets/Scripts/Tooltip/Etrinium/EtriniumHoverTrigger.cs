using UnityEngine;
using UnityEngine.EventSystems;

public class EtriniumHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HudController hudController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hudController?.ShowEtriniumTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hudController?.HideEtriniumTooltip();
    }
}