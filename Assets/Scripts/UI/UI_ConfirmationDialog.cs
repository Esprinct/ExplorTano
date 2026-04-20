using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConfirmationDialog : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text confirmButtonText;
    [SerializeField] private TMP_Text cancelButtonText;

    private Action onConfirm;

    private void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(HandleConfirm);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Close);

        Close();
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(HandleConfirm);

        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(Close);
    }

    public void Open(
        string message,
        Action onConfirmAction,
        string confirmLabel = "Confirmer",
        string cancelLabel = "Annuler")
    {
        onConfirm = onConfirmAction;

        if (messageText != null)
            messageText.text = message;

        if (confirmButtonText != null)
            confirmButtonText.text = confirmLabel;

        if (cancelButtonText != null)
            cancelButtonText.text = cancelLabel;

        if (panelRoot != null)
            panelRoot.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Close()
    {
        onConfirm = null;

        if (panelRoot != null)
            panelRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void HandleConfirm()
    {
        Action action = onConfirm;
        Close();
        action?.Invoke();
    }
}