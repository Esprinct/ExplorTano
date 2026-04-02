using UnityEngine;

public class MapController : MonoBehaviour
{
    private ProvinceView provinceSelectionnee;

    public void SelectionnerProvince(ProvinceView provinceView)
    {
        if (provinceSelectionnee == provinceView)
        {
            return;
        }

        if (provinceSelectionnee != null)
        {
            provinceSelectionnee.Deselectionner();
        }

        provinceSelectionnee = provinceView;

        if (provinceSelectionnee != null)
        {
            provinceSelectionnee.Selectionner();
        }
    }

    public ProvinceState GetProvinceStateSelectionnee()
    {
        return provinceSelectionnee != null ? provinceSelectionnee.ProvinceState : null;
    }

    public ProvinceView GetProvinceViewSelectionnee()
    {
        return provinceSelectionnee;
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
