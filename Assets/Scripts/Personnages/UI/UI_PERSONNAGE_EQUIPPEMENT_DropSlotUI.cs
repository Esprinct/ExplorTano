using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PERSONNAGE_EQUIPPEMENT_DropSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private ENUM_OBJET_EQUIPPABLE typeAccepte;

    private UI_PERSONNAGE_EQUIPPEMENT_PanelController controller;

    public void Setup(UI_PERSONNAGE_EQUIPPEMENT_PanelController panelController, ENUM_OBJET_EQUIPPABLE type)
    {
        controller = panelController;
        typeAccepte = type;
    }

    public void OnDrop(PointerEventData eventData)
    {
        SCOBJ_OBJET_EQUIPPABLE objet = EQUIPEMENT_DragContext.ObjetEnCours;

        if (objet == null || controller == null)
            return;

        if (objet.typeEquipable != typeAccepte)
        {
            Debug.Log($"Drop refusé : {objet.nom} n'est pas de type {typeAccepte}");
            return;
        }

        controller.TryEquipFromDrag(typeAccepte, objet);
    }
}