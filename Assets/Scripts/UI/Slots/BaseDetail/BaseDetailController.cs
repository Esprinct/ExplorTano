using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class BaseDetailController<TData> : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] protected GameObject panelRoot;

    [Header("Boutons")]
    [SerializeField] protected Button closeButton;
    [SerializeField] protected Button primaryActionButton;
    [SerializeField] protected TMP_Text primaryActionText;

    [Header("Malus / Bonus")]
    [SerializeField] protected Transform malusBonusContent;
    [SerializeField] protected UI_EFFET_Slot malusBonusSlotPrefab;

    protected TData currentData;

    protected virtual void Awake()
    {
        AutoBind();
        ValidateReferences();
        BindButtons();
        CloseMenu();
    }

    protected virtual void Update()
    {
        if (panelRoot != null &&
            panelRoot.activeSelf &&
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }
    }

    protected virtual void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag tag = GetComponentInChildren<PanelRootTag>(true);
            if (tag != null)
            {
                panelRoot = tag.gameObject;
            }
        }

        if (closeButton == null && panelRoot != null)
        {
            CloseButtonTag closeTag = panelRoot.GetComponentInChildren<CloseButtonTag>(true);
            if (closeTag != null)
            {
                closeButton = closeTag.GetComponent<Button>();
            }
        }
    }

    protected virtual void ValidateReferences()
    {
        UTIL_UiReferenceValidator.Require(panelRoot, nameof(panelRoot), this);
    }

    protected virtual void BindButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnClickClose);
            closeButton.onClick.AddListener(OnClickClose);
        }

        if (primaryActionButton != null)
        {
            primaryActionButton.onClick.RemoveListener(OnClickPrimaryAction);
            primaryActionButton.onClick.AddListener(OnClickPrimaryAction);
        }
    }

    public virtual void OpenMenu(TData data)
    {
        if (data == null)
        {
            Debug.LogWarning($"{GetType().Name} : data est null");
            return;
        }

        if (panelRoot == null)
        {
            Debug.LogError($"{GetType().Name} : panelRoot est null");
            return;
        }

        currentData = data;

        panelRoot.SetActive(true);
        panelRoot.transform.SetAsLastSibling();

        RefreshCurrentView();
    }

    public virtual void RefreshCurrentView()
    {
        if (currentData == null)
            return;

        RefreshUI(currentData);
        RefreshMalusBonus(currentData);
        RefreshPrimaryAction(currentData);
    }

    public virtual void CloseMenu()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public virtual void OnClickClose()
    {
        CloseMenu();
    }

    protected virtual void OnClickPrimaryAction()
    {
    }

    protected virtual void RefreshPrimaryAction(TData data)
    {
        if (primaryActionButton != null)
        {
            primaryActionButton.gameObject.SetActive(false);
            primaryActionButton.interactable = false;
        }

        if (primaryActionText != null)
        {
            primaryActionText.text = string.Empty;
        }
    }

    public bool IsOpen()
    {
        return panelRoot != null && panelRoot.activeInHierarchy;
    }

    public TData GetCurrentData()
    {
        return currentData;
    }

    protected virtual void RefreshMalusBonus(TData data)
    {
        if (malusBonusContent == null || malusBonusSlotPrefab == null)
            return;

        ClearMalusBonusContent();

        IReadOnlyList<SCOBJ_EFFET> effets = GetEffets(data);
        if (effets == null)
            return;

        ENUM_PERSONNAGE_Genre? genre = GetGenreForEffets(data);

        foreach (SCOBJ_EFFET effet in effets)
        {
            if (effet == null)
                continue;

            UI_EFFET_Slot slot = Instantiate(malusBonusSlotPrefab, malusBonusContent);
            slot.Setup(effet, genre);
        }
    }

    protected virtual void ClearMalusBonusContent()
    {
        if (malusBonusContent == null)
            return;

        for (int i = malusBonusContent.childCount - 1; i >= 0; i--)
        {
            Destroy(malusBonusContent.GetChild(i).gameObject);
        }
    }

    protected virtual ENUM_PERSONNAGE_Genre? GetGenreForEffets(TData data)
    {
        return null;
    }

    protected abstract IReadOnlyList<SCOBJ_EFFET> GetEffets(TData data);
    protected abstract void RefreshUI(TData data);

    protected virtual void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnClickClose);
        }

        if (primaryActionButton != null)
        {
            primaryActionButton.onClick.RemoveListener(OnClickPrimaryAction);
        }
    }
}