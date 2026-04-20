using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UTIL_UI_OBJET_SlotClickHandlerUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float doubleClickThreshold = 0.3f;

    private float lastClickTime = -10f;

    private Action onSingleClick;
    private Action onDoubleClick;

    public void Setup(Action singleClickAction, Action doubleClickAction)
    {
        onSingleClick = singleClickAction;
        onDoubleClick = doubleClickAction;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        float now = Time.unscaledTime;
        bool isDoubleClick = now - lastClickTime <= doubleClickThreshold;
        lastClickTime = now;

        if (isDoubleClick)
        {
            onDoubleClick?.Invoke();
            return;
        }

        onSingleClick?.Invoke();
    }
}