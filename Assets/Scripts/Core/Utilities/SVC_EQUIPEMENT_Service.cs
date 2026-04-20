public static class SVC_EQUIPEMENT_Service
{
    public static bool EquiperObjet(
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage,
        SCOBJ_OBJET_EQUIPPABLE objet)
    {
        return UTIL_JOUEUR_EQUIPPEMENT.EquiperObjetAuPersonnage(joueur, personnage, objet);
    }

    public static bool DesequiperObjet(
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage,
        ENUM_OBJET_EQUIPPABLE type)
    {
        return UTIL_JOUEUR_EQUIPPEMENT.DesequiperObjetDuPersonnage(joueur, personnage, type);
    }
}