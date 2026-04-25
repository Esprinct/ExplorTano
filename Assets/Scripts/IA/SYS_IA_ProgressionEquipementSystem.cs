using System.Collections.Generic;
using UnityEngine;

public class SYS_IA_ProgressionEquipementSystem
{
    public void OptimiserRosterIA(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return;

        AutoDistribuerPointsStats(joueur);
        AutoEquiperPersonnages(joueur);
    }

    private void AutoDistribuerPointsStats(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.personnagesRecrutes == null)
            return;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null || personnage.progression == null)
                continue;

            if (personnage.allocation == null)
                personnage.allocation = new STATE_STATS_Allocation();

            while (personnage.progression.pointsDisponibles > 0)
            {
                EffetENUM_Stats statCible = ChoisirStatPrioritaire(joueur, personnage);
                AjouterPoint(personnage, statCible);
                personnage.progression.pointsDisponibles--;
            }
        }
    }

    private EffetENUM_Stats ChoisirStatPrioritaire(DATA_JOUEUR joueur, SCOBJ_Personnage personnage)
    {
        if (joueur == null || personnage == null)
            return EffetENUM_Stats.Curiosite;

        switch (joueur.personnaliteIA)
        {
            case ENUM_IA_Personnalite.Agressive:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Curiosite,
                    EffetENUM_Stats.Endurance,
                    EffetENUM_Stats.Combativite,
                    EffetENUM_Stats.Ingeniosite
                );

            case ENUM_IA_Personnalite.Prestige:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Curiosite,
                    EffetENUM_Stats.Ingeniosite,
                    EffetENUM_Stats.Combativite,
                    EffetENUM_Stats.Endurance
                );

            case ENUM_IA_Personnalite.Economique:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Ingeniosite,
                    EffetENUM_Stats.Endurance,
                    EffetENUM_Stats.Combativite,
                    EffetENUM_Stats.Curiosite
                );

            case ENUM_IA_Personnalite.Expansionniste:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Combativite,
                    EffetENUM_Stats.Endurance,
                    EffetENUM_Stats.Ingeniosite,
                    EffetENUM_Stats.Curiosite
                );

            case ENUM_IA_Personnalite.Opportuniste:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Ingeniosite,
                    EffetENUM_Stats.Combativite,
                    EffetENUM_Stats.Curiosite,
                    EffetENUM_Stats.Endurance
                );

            case ENUM_IA_Personnalite.Equilibree:
            default:
                return ChoisirStatLaPlusFaible(personnage);
        }
    }

    private EffetENUM_Stats ChoisirStatSelonPriorites(
        SCOBJ_Personnage personnage,
        EffetENUM_Stats stat1,
        EffetENUM_Stats stat2,
        EffetENUM_Stats stat3,
        EffetENUM_Stats stat4)
    {
        int v1 = CALS_PERSONNAGE_STATS_Calculator.GetBaseStatAvecAllocation(personnage, stat1);
        int v2 = CALS_PERSONNAGE_STATS_Calculator.GetBaseStatAvecAllocation(personnage, stat2);
        int v3 = CALS_PERSONNAGE_STATS_Calculator.GetBaseStatAvecAllocation(personnage, stat3);
        int v4 = CALS_PERSONNAGE_STATS_Calculator.GetBaseStatAvecAllocation(personnage, stat4);

        int min = Mathf.Min(v1, Mathf.Min(v2, Mathf.Min(v3, v4)));

        if (v1 == min) return stat1;
        if (v2 == min) return stat2;
        if (v3 == min) return stat3;
        return stat4;
    }

    private EffetENUM_Stats ChoisirStatLaPlusFaible(SCOBJ_Personnage personnage)
    {
        return ChoisirStatSelonPriorites(
            personnage,
            EffetENUM_Stats.Curiosite,
            EffetENUM_Stats.Ingeniosite,
            EffetENUM_Stats.Combativite,
            EffetENUM_Stats.Endurance
        );
    }

    private void AjouterPoint(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null)
            return;

        if (personnage.allocation == null)
            personnage.allocation = new STATE_STATS_Allocation();

        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                personnage.allocation.curiosite++;
                break;
            case EffetENUM_Stats.Ingeniosite:
                personnage.allocation.ingeniosite++;
                break;
            case EffetENUM_Stats.Combativite:
                personnage.allocation.combativite++;
                break;
            case EffetENUM_Stats.Endurance:
                personnage.allocation.endurance++;
                break;
        }
    }

    private void AutoEquiperPersonnages(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.personnagesRecrutes == null || joueur.objetsPossedes == null)
            return;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            AutoEquiperType(joueur, personnage, ENUM_OBJET_EQUIPPABLE.Outil);
            AutoEquiperType(joueur, personnage, ENUM_OBJET_EQUIPPABLE.Tenue);
            AutoEquiperType(joueur, personnage, ENUM_OBJET_EQUIPPABLE.Accessoire);
        }
    }

    private void AutoEquiperType(DATA_JOUEUR joueur, SCOBJ_Personnage personnage, ENUM_OBJET_EQUIPPABLE type)
    {
        if (joueur == null || personnage == null || joueur.objetsPossedes == null)
            return;

        SCOBJ_OBJET_EQUIPPABLE objetActuel = UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnage, type);
        float scoreActuel = EvaluerObjetPourPersonnage(objetActuel, joueur, personnage);

        SCOBJ_OBJET_EQUIPPABLE meilleurObjet = objetActuel;
        float meilleurScore = scoreActuel;

        foreach (SCOBJ_OBJET objet in joueur.objetsPossedes)
        {
            SCOBJ_OBJET_EQUIPPABLE equipable = objet as SCOBJ_OBJET_EQUIPPABLE;
            if (equipable == null)
                continue;

            if (equipable.typeEquipable != type)
                continue;

            if (!UTIL_PERSONNAGE_EQUIPEMENT.PeutEquiper(personnage, equipable))
                continue;

            if (SVC_OBJET_EquipementRequeteService.EstEquipeParUnDesPersonnagesDuJoueur(joueur, equipable))
                continue;

            float score = EvaluerObjetPourPersonnage(equipable, joueur, personnage);
            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleurObjet = equipable;
            }
        }

        if (meilleurObjet != null && meilleurObjet != objetActuel)
        {
            UTIL_JOUEUR_EQUIPPEMENT.EquiperObjetAuPersonnage(joueur, personnage, meilleurObjet);
        }
    }

    private float EvaluerObjetPourPersonnage(
        SCOBJ_OBJET_EQUIPPABLE objet,
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage)
    {
        if (objet == null || joueur == null || personnage == null)
            return 0f;

        int curiositeAvant = CALS_PERSONNAGE_STATS_Calculator.GetCuriositeEffective(personnage, joueur.compagnie);
        int ingeniositeAvant = CALS_PERSONNAGE_STATS_Calculator.GetIngeniositeEffective(personnage, joueur.compagnie);
        int combativiteAvant = CALS_PERSONNAGE_STATS_Calculator.GetCombativiteEffective(personnage, joueur.compagnie);
        int enduranceAvant = CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, joueur.compagnie);

        SCOBJ_OBJET_EQUIPPABLE ancien = UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnage, objet.typeEquipable);

        if (ancien != null)
            personnage.objetsEquipes.Remove(ancien);

        personnage.objetsEquipes ??= new List<SCOBJ_OBJET_EQUIPPABLE>();
        personnage.objetsEquipes.Add(objet);

        int curiositeApres = CALS_PERSONNAGE_STATS_Calculator.GetCuriositeEffective(personnage, joueur.compagnie);
        int ingeniositeApres = CALS_PERSONNAGE_STATS_Calculator.GetIngeniositeEffective(personnage, joueur.compagnie);
        int combativiteApres = CALS_PERSONNAGE_STATS_Calculator.GetCombativiteEffective(personnage, joueur.compagnie);
        int enduranceApres = CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, joueur.compagnie);

        personnage.objetsEquipes.Remove(objet);
        if (ancien != null)
            personnage.objetsEquipes.Add(ancien);

        int deltaCuriosite = curiositeApres - curiositeAvant;
        int deltaIngeniosite = ingeniositeApres - ingeniositeAvant;
        int deltaCombativite = combativiteApres - combativiteAvant;
        int deltaEndurance = enduranceApres - enduranceAvant;

        switch (joueur.personnaliteIA)
        {
            case ENUM_IA_Personnalite.Agressive:
                return deltaCuriosite * 2.0f + deltaEndurance * 1.5f + deltaCombativite * 0.8f + deltaIngeniosite * 0.3f;

            case ENUM_IA_Personnalite.Prestige:
                return deltaCuriosite * 1.6f + deltaIngeniosite * 1.2f + deltaCombativite * 0.6f + deltaEndurance * 0.5f;

            case ENUM_IA_Personnalite.Economique:
                return deltaIngeniosite * 1.6f + deltaEndurance * 1.3f + deltaCombativite * 0.7f + deltaCuriosite * 0.4f;

            case ENUM_IA_Personnalite.Expansionniste:
                return deltaCombativite * 1.8f + deltaEndurance * 1.1f + deltaIngeniosite * 0.8f + deltaCuriosite * 0.6f;

            case ENUM_IA_Personnalite.Opportuniste:
                return deltaIngeniosite * 1.2f + deltaCombativite * 1.2f + deltaCuriosite * 0.9f + deltaEndurance * 0.9f;

            case ENUM_IA_Personnalite.Equilibree:
            default:
                return deltaCuriosite + deltaIngeniosite + deltaCombativite + deltaEndurance;
        }
    }
}