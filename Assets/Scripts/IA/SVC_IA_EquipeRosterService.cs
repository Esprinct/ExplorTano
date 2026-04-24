using System.Collections.Generic;
using UnityEngine;

public static class SVC_IA_EquipeRosterService
{
    private const int TailleMaxEquipe = 12;

 public static void TenterCreationEquipe(SYS_GameManager gameManager, DATA_JOUEUR joueur)
{
    if (gameManager == null || joueur == null)
        return;

    int personnagesLibres = CompterPersonnagesLibres(gameManager, joueur);
    int nbEquipes = GetNombreEquipesValides(joueur);

    int nombreEquipesMaximumSouhaite = SVC_IA_EquipeCompositionService.GetNombreEquipesMaximumSouhaite(joueur);
    bool besoinStructurel = SVC_IA_EquipeCompositionService.IADevraitCreerNouvelleEquipe(joueur);

    if (nbEquipes >= nombreEquipesMaximumSouhaite)
        return;

    if (nbEquipes >= gameManager.MaxEquipesParJoueur)
        return;

    if (personnagesLibres <= 0)
        return;

    // Plus agressif : si l'IA veut encore des équipes, elle en crée dès qu'elle a au moins 1 perso libre.
    bool doitCreerEquipe =
        nbEquipes == 0 ||
        besoinStructurel ||
        personnagesLibres >= GetTailleCibleEquipe(joueur);

    if (!doitCreerEquipe)
        return;

    if (!gameManager.PeutCreerEquipe(joueur))
        return;

    STATE_EQUIPE nouvelleEquipe = ConstruireNouvelleEquipe(gameManager, joueur);
    if (nouvelleEquipe == null)
        return;

    int coutCreation = gameManager.GetCoutCreationEquipe(joueur);
    joueur.etrinium -= coutCreation;

    gameManager.EquipesRuntime.Add(nouvelleEquipe);

    joueur.equipes ??= new List<STATE_EQUIPE>();
    joueur.equipes.Add(nouvelleEquipe);

    Debug.Log(
        $"[IA_EQUIPE_CREATE] joueur={joueur.nomJoueur} | nbEquipes={joueur.equipes.Count} | " +
        $"persosLibresRestantsAvantAssignation={personnagesLibres} | cout={coutCreation}"
    );
}

    public static void CompleterEquipes(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        List<SCOBJ_Personnage> personnagesLibres = RecupererPersonnagesLibres(gameManager, joueur);
        if (personnagesLibres.Count == 0)
            return;

        int tailleCibleEquipe = GetTailleCibleEquipe(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null || equipe.AUneActionEnCours)
                continue;

            equipe.membresActuels ??= new List<SCOBJ_Personnage>();
            equipe.membresActuels.RemoveAll(p => p == null);

            while (equipe.membresActuels.Count < tailleCibleEquipe &&
                   equipe.membresActuels.Count < TailleMaxEquipe &&
                   personnagesLibres.Count > 0)
            {
                SCOBJ_Personnage meilleur = ChoisirMeilleurPersonnagePourEquipe(personnagesLibres, joueur);
                if (meilleur == null)
                    break;

                equipe.membresActuels.Add(meilleur);
                personnagesLibres.Remove(meilleur);
            }
        }
    }

    public static int GetTailleMinEquipePourAction(DATA_JOUEUR joueur)
    {
        return SVC_IA_PersonnaliteResolver.GetTailleMinEquipePourAction(joueur);
    }

    private static STATE_EQUIPE ConstruireNouvelleEquipe(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return null;

        SCOBJ_EQUIPE dataEquipe = ScriptableObject.CreateInstance<SCOBJ_EQUIPE>();
        if (dataEquipe == null)
            return null;

        int index = GetNombreEquipesValides(joueur) + 1;

        dataEquipe.name = $"Equipe_IA_{joueur.compagnie}_{index}";
        dataEquipe.nomEquipe = $"Équipe {joueur.compagnie} {index}";
        dataEquipe.niveauDeBase = 1;
        dataEquipe.membres = new List<SCOBJ_Personnage>();

        return new STATE_EQUIPE
        {
            data = dataEquipe,
            compagnie = joueur.compagnie,
            niveauActuel = 1,
            provinceAffectee = null,
            actionEnCours = ENUM_EQUIPE_ACTION.Aucune,
            actionTerminee = false,
            actionToursRestants = 0,
            actionToursTotaux = 0,
            membresActuels = new List<SCOBJ_Personnage>(),
            affectationAutomatique = true,
            lancementActionAutomatique = true,
            objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
            consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),
            progression = new STATE_LevelProgression(),
            progressionConfig = gameManager.ProgressionConfigEquipe
        };
    }

    private static int GetNombreEquipesValides(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe != null)
                total++;
        }

        return total;
    }

    private static int CompterPersonnagesLibres(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        return RecupererPersonnagesLibres(gameManager, joueur).Count;
    }

    private static List<SCOBJ_Personnage> RecupererPersonnagesLibres(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        List<SCOBJ_Personnage> resultat = new();

        if (gameManager == null || joueur == null || joueur.personnagesRecrutes == null)
            return resultat;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            if (EstPersonnageDejaAffecte(joueur, personnage))
                continue;

            resultat.Add(personnage);
        }

        return resultat;
    }

    private static bool EstPersonnageDejaAffecte(DATA_JOUEUR joueur, SCOBJ_Personnage personnage)
    {
        if (joueur == null || personnage == null || joueur.equipes == null)
            return false;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null || equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Contains(personnage))
                return true;
        }

        return false;
    }

    private static SCOBJ_Personnage ChoisirMeilleurPersonnagePourEquipe(List<SCOBJ_Personnage> candidats, DATA_JOUEUR joueur)
    {
        if (candidats == null || candidats.Count == 0 || joueur == null)
            return null;

        SCOBJ_Personnage meilleur = null;
        float meilleurScore = float.MinValue;

        foreach (SCOBJ_Personnage personnage in candidats)
        {
            if (personnage == null)
                continue;

            float score = SVC_IA_PersonnaliteResolver.EvaluerPersonnage(personnage, joueur);

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleur = personnage;
            }
        }

        return meilleur;
    }

    private static int GetTailleCibleEquipe(DATA_JOUEUR joueur)
    {
        return SVC_IA_PersonnaliteResolver.GetTailleCibleEquipe(joueur);
    }
}