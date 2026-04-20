using System.Collections.Generic;

public class DATA_RecrutementResolutionResult
{
    public List<DATA_RecrutementNotificationItem> notifications = new();

    public bool ADesNotifications()
    {
        return notifications != null && notifications.Count > 0;
    }
}