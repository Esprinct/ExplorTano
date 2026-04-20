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
            return EffetENUM_Stats.Force;

        switch (joueur.personnaliteIA)
        {
            case ENUM_IA_Personnalite.Agressive:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Force,
                    EffetENUM_Stats.Endurance,
                    EffetENUM_Stats.Dexterite,
                    EffetENUM_Stats.Intelligence
                );

            case ENUM_IA_Personnalite.Prestige:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Force,
                    EffetENUM_Stats.Intelligence,
                    EffetENUM_Stats.Dexterite,
                    EffetENUM_Stats.Endurance
                );

            case ENUM_IA_Personnalite.Economique:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Intelligence,
                    EffetENUM_Stats.Endurance,
                    EffetENUM_Stats.Dexterite,
                    EffetENUM_Stats.Force
                );

            case ENUM_IA_Personnalite.Expansionniste:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Dexterite,
                    EffetENUM_Stats.Endurance,
                    EffetENUM_Stats.Intelligence,
                    EffetENUM_Stats.Force
                );

            case ENUM_IA_Personnalite.Opportuniste:
                return ChoisirStatSelonPriorites(
                    personnage,
                    EffetENUM_Stats.Intelligence,
                    EffetENUM_Stats.Dexterite,
                    EffetENUM_Stats.Force,
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
            EffetENUM_Stats.Force,
            EffetENUM_Stats.Intelligence,
            EffetENUM_Stats.Dexterite,
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
            case EffetENUM_Stats.Force:
                personnage.allocation.force++;
                break;
            case EffetENUM_Stats.Intelligence:
                personnage.allocation.intelligence++;
                break;
            case EffetENUM_Stats.Dexterite:
                personnage.allocation.dexterite++;
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

        int forceAvant = CALS_PERSONNAGE_STATS_Calculator.GetForceEffective(personnage, joueur.compagnie);
        int intelligenceAvant = CALS_PERSONNAGE_STATS_Calculator.GetIntelligenceEffective(personnage, joueur.compagnie);
        int dexteriteAvant = CALS_PERSONNAGE_STATS_Calculator.GetDexteriteEffective(personnage, joueur.compagnie);
        int enduranceAvant = CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, joueur.compagnie);

        SCOBJ_OBJET_EQUIPPABLE ancien = UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnage, objet.typeEquipable);

        if (ancien != null)
            personnage.objetsEquipes.Remove(ancien);

        personnage.objetsEquipes ??= new List<SCOBJ_OBJET_EQUIPPABLE>();
        personnage.objetsEquipes.Add(objet);

        int forceApres = CALS_PERSONNAGE_STATS_Calculator.GetForceEffective(personnage, joueur.compagnie);
        int intelligenceApres = CALS_PERSONNAGE_STATS_Calculator.GetIntelligenceEffective(personnage, joueur.compagnie);
        int dexteriteApres = CALS_PERSONNAGE_STATS_Calculator.GetDexteriteEffective(personnage, joueur.compagnie);
        int enduranceApres = CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, joueur.compagnie);

        personnage.objetsEquipes.Remove(objet);
        if (ancien != null)
            personnage.objetsEquipes.Add(ancien);

        int deltaForce = forceApres - forceAvant;
        int deltaIntelligence = intelligenceApres - intelligenceAvant;
        int deltaDexterite = dexteriteApres - dexteriteAvant;
        int deltaEndurance = enduranceApres - enduranceAvant;

        switch (joueur.personnaliteIA)
        {
            case ENUM_IA_Personnalite.Agressive:
                return deltaForce * 2.0f + deltaEndurance * 1.5f + deltaDexterite * 0.8f + deltaIntelligence * 0.3f;

            case ENUM_IA_Personnalite.Prestige:
                return deltaForce * 1.6f + deltaIntelligence * 1.2f + deltaDexterite * 0.6f + deltaEndurance * 0.5f;

            case ENUM_IA_Personnalite.Economique:
                return deltaIntelligence * 1.6f + deltaEndurance * 1.3f + deltaDexterite * 0.7f + deltaForce * 0.4f;

            case ENUM_IA_Personnalite.Expansionniste:
                return deltaDexterite * 1.8f + deltaEndurance * 1.1f + deltaIntelligence * 0.8f + deltaForce * 0.6f;

            case ENUM_IA_Personnalite.Opportuniste:
                return deltaIntelligence * 1.2f + deltaDexterite * 1.2f + deltaForce * 0.9f + deltaEndurance * 0.9f;

            case ENUM_IA_Personnalite.Equilibree:
            default:
                return deltaForce + deltaIntelligence + deltaDexterite + deltaEndurance;
        }
    }
}