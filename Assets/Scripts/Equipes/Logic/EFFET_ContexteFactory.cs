public static class EFFET_ContexteFactory
{
    public static EFFET_Contexte ForEquipe(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur = null,
        STATE_PROVINCE province = null)
    {
        if (equipe == null)
            return null;

        return new EFFET_Contexte
        {
            personnage = null,
            compagnie = equipe.compagnie,
            STATE_PERSONNAGE = null,
            equipe = equipe,
            joueur = joueur,
            province = province ?? equipe.provinceAffectee
        };
    }
}