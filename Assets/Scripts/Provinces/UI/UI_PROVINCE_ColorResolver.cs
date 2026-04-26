using UnityEngine;

public static class UI_PROVINCE_ColorResolver
{
    public static Color GetCouleurBase(
        STATE_PROVINCE province,
        Color couleurMaizin,
        Color couleurKinia,
        Color couleurJoho,
        Color couleurAutre)
    {
        if (province == null || !province.proprietaireActuel.HasValue)
            return couleurAutre;

        switch (province.proprietaireActuel.Value)
        {
            case ENUM_Compagnie.Maizin:
                return couleurMaizin;

            case ENUM_Compagnie.Kinia:
                return couleurKinia;

            case ENUM_Compagnie.Joho:
                return couleurJoho;

            default:
                return couleurAutre;
        }
    }
}