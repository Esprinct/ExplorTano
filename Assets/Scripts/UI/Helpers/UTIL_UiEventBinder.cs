using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UTIL_UiEventBinder
{
    public static void Bind(Button button, UnityAction action, MonoBehaviour owner, string fieldName)
    {
        if (button == null)
        {
            Debug.LogWarning($"[{owner.GetType().Name}] {fieldName} est null dans {owner.name}");
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    public static void Bind(Toggle toggle, UnityAction<bool> action, MonoBehaviour owner, string fieldName)
    {
        if (toggle == null)
        {
            Debug.LogWarning($"[{owner.GetType().Name}] {fieldName} est null dans {owner.name}");
            return;
        }

        toggle.onValueChanged.RemoveListener(action);
        toggle.onValueChanged.AddListener(action);
    }

    public static void Unbind(Button button, UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveListener(action);
        }
    }

    public static void Unbind(Toggle toggle, UnityAction<bool> action)
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(action);
        }
    }
}