using UnityEngine;

public static class UI_PROVINCE_ExplorationContourResolver
{
    private const float SeuilExplorationComplete = 99.999f;

    public static bool TryGetCouleurContour(
        STATE_PROVINCE province,
        Color couleurMaizin,
        Color couleurKinia,
        Color couleurJoho,
        out Color couleurContour)
    {
        couleurContour = Color.clear;

        if (province == null)
            return false;

        bool maizinComplete = EstComplete(province, ENUM_Compagnie.Maizin);
        bool kiniaComplete = EstComplete(province, ENUM_Compagnie.Kinia);
        bool johoComplete = EstComplete(province, ENUM_Compagnie.Joho);

        if (maizinComplete && kiniaComplete && johoComplete)
        {
            couleurContour = Color.white;
            return true;
        }

        if (maizinComplete)
        {
            couleurContour = couleurMaizin;
            return true;
        }

        if (kiniaComplete)
        {
            couleurContour = couleurKinia;
            return true;
        }

        if (johoComplete)
        {
            couleurContour = couleurJoho;
            return true;
        }

        return false;
    }

    private static bool EstComplete(STATE_PROVINCE province, ENUM_Compagnie compagnie)
    {
        if (province == null)
            return false;

        return province.GetExploration(compagnie) >= SeuilExplorationComplete;
    }
}