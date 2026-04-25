using System.Collections.Generic;
using UnityEngine;

public static class MAP_PERSONNAGE_DetailMapper
{
    public static DATA_PERSONNAGE_Detail ToHudData(SCOBJ_Personnage data, DATA_PERSONNAGE_DisplayContext contexte)
    {
        if (contexte == null)
            contexte = DATA_PERSONNAGE_DisplayContext.Default;

        return ToHudData(data, contexte.compagnie, contexte.state);
    }

    public static DATA_PERSONNAGE_Detail ToHudData(
        SCOBJ_Personnage data,
        ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune,
        STATE_PERSONNAGE state = null)
    {
        if (data == null)
            return null;

        CALC_PERSONNAGE_STATS_ComputedStats stats = SVC_PERSONNAGE_STATS.Compute(data, compagnie, state);

        DATA_LevelProgressionView progressionView = null;

        if (data.progression != null && data.progressionConfig != null)
        {
            progressionView = MAP_XP_LevelProgression.ToViewData(
                data.progression,
                data.progressionConfig
            );
        }

        List<SCOBJ_EFFET> effetsAffiches = new();

        if (data.effets != null)
            effetsAffiches.AddRange(data.effets);

        effetsAffiches.AddRange(UTIL_PERSONNNAGE_EQUIPEMENT_EFFET.GetEffetsEquipements(data));

        return new DATA_PERSONNAGE_Detail
        {
            nom = data.nom,
            prenom = data.prenom,
            sprite = data.sprite,
            progression = progressionView,
            role = data.roleActuel.ToString(),
            description = data.description,
            genre = data.genre,
            rareteEtoiles = data.rareteEtoiles,

            curiositeBase = stats.curiosite.baseValue,
            intelligenceBase = stats.intelligence.baseValue,
            dexteriteBase = stats.dexterite.baseValue,
            enduranceBase = stats.endurance.baseValue,

            curiosite = stats.curiosite.finalValue,
            intelligence = stats.intelligence.finalValue,
            dexterite = stats.dexterite.finalValue,
            endurance = stats.endurance.finalValue,

            curiositeDelta = stats.curiosite.delta,
            intelligenceDelta = stats.intelligence.delta,
            dexteriteDelta = stats.dexterite.delta,
            enduranceDelta = stats.endurance.delta,

            curiositeTooltipDetail = string.Join("\n", stats.curiosite.detailLines),
            intelligenceTooltipDetail = string.Join("\n", stats.intelligence.detailLines),
            dexteriteTooltipDetail = string.Join("\n", stats.dexterite.detailLines),
            enduranceTooltipDetail = string.Join("\n", stats.endurance.detailLines),

            coutParTour = data.coutParTour,
            idUnique = data.idUnique,
            effets = effetsAffiches
        };
    }
}