using UnityEngine;

public enum ENUM_PROVINCE_ExplorationContourMode
{
    Aucun,
    CouleurSimple,
    Hachure,
    Blanc
}

public struct DATA_PROVINCE_ExplorationContourStyle
{
    public ENUM_PROVINCE_ExplorationContourMode mode;
    public Color couleurA;
    public Color couleurB;
}

public static class UI_PROVINCE_ExplorationContourResolver
{
    private const float SeuilExplorationComplete = 99.999f;

    public static bool TryGetStyleContour(
        STATE_PROVINCE province,
        Color couleurMaizin,
        Color couleurKinia,
        Color couleurJoho,
        out DATA_PROVINCE_ExplorationContourStyle style)
    {
        style = new DATA_PROVINCE_ExplorationContourStyle
        {
            mode = ENUM_PROVINCE_ExplorationContourMode.Aucun,
            couleurA = Color.clear,
            couleurB = Color.clear
        };

        if (province == null)
            return false;

        bool maizinComplete = EstComplete(province, ENUM_Compagnie.Maizin);
        bool kiniaComplete = EstComplete(province, ENUM_Compagnie.Kinia);
        bool johoComplete = EstComplete(province, ENUM_Compagnie.Joho);

        int totalComplets = 0;
        if (maizinComplete) totalComplets++;
        if (kiniaComplete) totalComplets++;
        if (johoComplete) totalComplets++;

        if (totalComplets <= 0)
            return false;

        if (totalComplets == 3)
        {
            style.mode = ENUM_PROVINCE_ExplorationContourMode.Blanc;
            style.couleurA = Color.white;
            style.couleurB = Color.white;
            return true;
        }

        if (totalComplets == 2)
        {
            style.mode = ENUM_PROVINCE_ExplorationContourMode.Hachure;

            if (maizinComplete && kiniaComplete)
            {
                style.couleurA = couleurMaizin;
                style.couleurB = couleurKinia;
                return true;
            }

            if (maizinComplete && johoComplete)
            {
                style.couleurA = couleurMaizin;
                style.couleurB = couleurJoho;
                return true;
            }

            if (kiniaComplete && johoComplete)
            {
                style.couleurA = couleurKinia;
                style.couleurB = couleurJoho;
                return true;
            }
        }

        style.mode = ENUM_PROVINCE_ExplorationContourMode.CouleurSimple;

        if (maizinComplete)
        {
            style.couleurA = couleurMaizin;
            style.couleurB = couleurMaizin;
            return true;
        }

        if (kiniaComplete)
        {
            style.couleurA = couleurKinia;
            style.couleurB = couleurKinia;
            return true;
        }

        if (johoComplete)
        {
            style.couleurA = couleurJoho;
            style.couleurB = couleurJoho;
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