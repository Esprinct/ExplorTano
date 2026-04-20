public static class EQUIPEMENT_DragContext
{
    public static SCOBJ_OBJET_EQUIPPABLE ObjetEnCours;
    public static UI_PERSONNAGE_EQUIPEMENT_Draggable SourceUI;

    // true = l'objet vient d'un slot équipé à gauche
    public static bool VientEquipement;

    // utile pour savoir quoi déséquiper
    public static ENUM_OBJET_EQUIPPABLE? TypeEquipementSource;

    public static void Clear()
    {
        ObjetEnCours = null;
        SourceUI = null;
        VientEquipement = false;
        TypeEquipementSource = null;
    }
}