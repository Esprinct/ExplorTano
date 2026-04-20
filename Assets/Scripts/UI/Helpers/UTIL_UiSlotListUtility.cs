using System.Collections.Generic;
using UnityEngine;

public static class UTIL_UiSlotListUtility
{
    public static void ClearSlots<T>(List<T> slots) where T : MonoBehaviour
    {
        if (slots == null)
            return;

        foreach (T slot in slots)
        {
            if (slot != null)
            {
                Object.Destroy(slot.gameObject);
            }
        }

        slots.Clear();
    }

    public static void PrepareTemplate(MonoBehaviour template)
    {
        if (template != null)
        {
            template.gameObject.SetActive(false);
        }
    }

    public static T CreateSlot<T>(T template, Transform parent) where T : MonoBehaviour
    {
        T slot = Object.Instantiate(template, parent);
        slot.gameObject.SetActive(true);
        return slot;
    }
}