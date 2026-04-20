using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PERSONNAGE_EQUIPPEMENT_DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_PanelController controller;

    public void Setup(UI_PERSONNAGE_EQUIPPEMENT_PanelController panelController)
    {
        controller = panelController;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (controller == null)
            return;

        SCOBJ_OBJET_EQUIPPABLE objet = EQUIPEMENT_DragContext.ObjetEnCours;
        if (objet == null)
            return;

        if (!EQUIPEMENT_DragContext.VientEquipement)
            return;

        if (!EQUIPEMENT_DragContext.TypeEquipementSource.HasValue)
            return;

        controller.TryDesequipFromDrag(EQUIPEMENT_DragContext.TypeEquipementSource.Value);
    }
}