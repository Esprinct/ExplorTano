using UnityEngine;

public abstract class UTIL_UiPanelControllerBase : MonoBehaviour
{
    [SerializeField] protected GameObject panelRoot;

    public virtual void OpenPanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    public virtual void ClosePanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public bool IsOpen()
    {
        return panelRoot != null && panelRoot.activeInHierarchy;
    }
}