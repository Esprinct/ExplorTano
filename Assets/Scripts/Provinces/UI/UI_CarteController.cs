using UnityEngine;

public class MapController : MonoBehaviour
{
    private UI_PROVINCE_View provinceSelectionnee;

    public void SelectionnerProvince(UI_PROVINCE_View UI_PROVINCE_View)
    {
        if (UI_PROVINCE_View == null)
        {
            ClearSelection();
            return;
        }

        if (provinceSelectionnee == UI_PROVINCE_View)
        {
            provinceSelectionnee.RefreshVisual();
            return;
        }

        if (provinceSelectionnee != null)
        {
            provinceSelectionnee.Deselectionner();
        }

        provinceSelectionnee = UI_PROVINCE_View;
        provinceSelectionnee.Selectionner();
    }

    public STATE_PROVINCE GetSTATE_PROVINCESelectionnee()
    {
        return provinceSelectionnee != null ? provinceSelectionnee.STATE_PROVINCE : null;
    }

    public UI_PROVINCE_View GetUI_PROVINCE_ViewSelectionnee()
    {
        return provinceSelectionnee;
    }

    public bool AUneProvinceSelectionnee()
    {
        return provinceSelectionnee != null;
    }

    public void ClearSelection()
    {
        if (provinceSelectionnee != null)
        {
            provinceSelectionnee.Deselectionner();
            provinceSelectionnee = null;
        }
    }
}