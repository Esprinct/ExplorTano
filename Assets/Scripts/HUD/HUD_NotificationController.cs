 
 using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_NotificationController : MonoBehaviour {
        [Header("Notification recrutement")]
    [SerializeField] private GameObject panelNotificationRecrutement;
    [SerializeField] private Transform notificationRecrutementContent;
    [SerializeField] private UI_Recrutement_NotificationItem notificationRecrutementItemPrefab;
    [SerializeField] private Button boutonFermerNotificationRecrutement;
 private readonly List<UI_Recrutement_NotificationItem> notificationsInstanciees = new();
public void ShowNotificationRecrutement(DATA_RecrutementResolutionResult resultat)
    {
        ClearNotificationRecrutementItems();

        if (resultat == null || !resultat.ADesNotifications())
            return;

        if (notificationRecrutementContent == null || notificationRecrutementItemPrefab == null)
        {
            Debug.LogWarning("Notification recrutement content/prefab non assigné.");
            return;
        }

        foreach (DATA_RecrutementNotificationItem notification in resultat.notifications)
        {
            if (notification == null)
                continue;

            UI_Recrutement_NotificationItem item =
                Instantiate(notificationRecrutementItemPrefab, notificationRecrutementContent);

            item.Refresh(notification);
            notificationsInstanciees.Add(item);
        }

        if (panelNotificationRecrutement != null)
            panelNotificationRecrutement.SetActive(true);
    }

    public void HideNotificationRecrutement()
    {
        if (panelNotificationRecrutement != null)
            panelNotificationRecrutement.SetActive(false);

        ClearNotificationRecrutementItems();
    }

    private void ClearNotificationRecrutementItems()
    {
        foreach (UI_Recrutement_NotificationItem item in notificationsInstanciees)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        notificationsInstanciees.Clear();
    }}