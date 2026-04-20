using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSlotUI<TData> : MonoBehaviour
{
    [Header("Commun")]
    [SerializeField] protected Button button;

    protected TData currentData;
    private Action<TData> onClickAction;

    protected virtual void Awake()
    {
        AutoBind();
        ValidateReferences();
        BindButton();
    }

    protected virtual void AutoBind()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>(true);
        }
    }

    protected virtual void ValidateReferences()
    {
        UTIL_UiReferenceValidator.Require(button, nameof(button), this);
    }

    protected virtual void BindButton()
    {
        if (button == null)
        {
            Debug.LogWarning($"Aucun Button trouvé dans {name}");
            return;
        }

        button.onClick.RemoveListener(OnClickSlot);
        button.onClick.AddListener(OnClickSlot);
    }

    public void SetOnClick(Action<TData> callback)
    {
        onClickAction = callback;
    }
protected virtual void OnClickSlot()
{
    Debug.Log($"{GetType().Name} click | currentData null ? {currentData == null}");

    if (currentData == null)
    {
        Debug.LogWarning($"{GetType().Name} : currentData est null");
        return;
    }

    Debug.Log($"{GetType().Name} invoke callback");
    onClickAction?.Invoke(currentData);
}
    public virtual void Refresh(TData data)
    {
        currentData = data;

        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        RefreshVisuals(data);

        if (button != null)
        {
            button.interactable = true;
        }
    }

    protected abstract void RefreshVisuals(TData data);

  
    protected virtual void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClickSlot);
        }
    }
}