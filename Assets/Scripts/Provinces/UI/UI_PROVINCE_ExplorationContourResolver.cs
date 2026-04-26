using System.Collections.Generic;

public static class UI_PROVINCE_ExplorationContourResolver
{
    private const float InfluenceMin = 0.001f;

    public static List<ENUM_Compagnie> GetCompagniesExploratrices(STATE_PROVINCE province)
    {
        List<ENUM_Compagnie> result = new();

        if (province == null)
            return result;

        if (province.estClaim)
            return result;

        List<KeyValuePair<ENUM_Compagnie, float>> influences = new();

        AjouterSiPositive(influences, ENUM_Compagnie.Maizin, province.influenceMaizin);
        AjouterSiPositive(influences, ENUM_Compagnie.Kinia, province.influenceKinia);
        AjouterSiPositive(influences, ENUM_Compagnie.Joho, province.influenceJoho);

        influences.Sort((a, b) => b.Value.CompareTo(a.Value));

        foreach (KeyValuePair<ENUM_Compagnie, float> kvp in influences)
        {
            result.Add(kvp.Key);
        }

        return result;
    }

    private static void AjouterSiPositive(
        List<KeyValuePair<ENUM_Compagnie, float>> influences,
        ENUM_Compagnie compagnie,
        float valeur)
    {
        if (valeur > InfluenceMin)
        {
            influences.Add(new KeyValuePair<ENUM_Compagnie, float>(compagnie, valeur));
        }
    }
}