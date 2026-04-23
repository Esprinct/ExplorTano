using UnityEngine;

public class SYS_FailliteSystem
{
    public void ResoudreFaillites(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return;

        ResoudreFailliteJoueur(gameManager, gameManager.Joueur1);
        ResoudreFailliteJoueur(gameManager, gameManager.Joueur2);
        ResoudreFailliteJoueur(gameManager, gameManager.Joueur3);
    }

    private void ResoudreFailliteJoueur(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return;

        if (joueur.personnagesRecrutes == null || joueur.personnagesRecrutes.Count == 0)
            return;

        RecalculerRevenuParTourPourJoueur(gameManager, joueur);

        if (joueur.etrinium >= 0f)
            return;

        Debug.Log(
            $"[FAILLITE] Début résolution | joueur={joueur.nomJoueur} | " +
            $"etrinium={joueur.etrinium} | revenu/tour={joueur.etriniumParTour}"
        );

        int securite = 0;
        const int maxIterations = 100;

        while (joueur.etrinium < 0f &&
               joueur.personnagesRecrutes.Count > 0 &&
               securite < maxIterations)
        {
            SCOBJ_Personnage personnageASupprimer = ChoisirPersonnageASupprimer(joueur);
            if (personnageASupprimer == null)
                break;

            SupprimerPersonnageDuJoueur(joueur, personnageASupprimer);
            InterrompreEquipesVides(joueur);
            RecalculerRevenuParTourPourJoueur(gameManager, joueur);

            securite++;
        }

        InterrompreEquipesVides(joueur);

        Debug.Log(
            $"[FAILLITE] Fin résolution | joueur={joueur.nomJoueur} | " +
            $"etrinium={joueur.etrinium} | revenu/tour={joueur.etriniumParTour} | " +
            $"persos restants={(joueur.personnagesRecrutes != null ? joueur.personnagesRecrutes.Count : 0)}"
        );
    }

    private SCOBJ_Personnage ChoisirPersonnageASupprimer(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.personnagesRecrutes == null)
            return null;

        SCOBJ_Personnage meilleurCandidat = null;
        int meilleurScore = int.MinValue;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            bool estDansEquipe = EstPersonnageDansUneEquipeDuJoueur(joueur, personnage);

            int coutNormal = SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);
            int score = coutNormal;

            if (!estDansEquipe)
                score += 100000;

            score += personnage.coutParTour * 10;
            score += personnage.rareteEtoiles * 100;

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleurCandidat = personnage;
            }
        }

        return meilleurCandidat;
    }

    private void SupprimerPersonnageDuJoueur(DATA_JOUEUR joueur, SCOBJ_Personnage personnage)
    {
        if (joueur == null || personnage == null)
            return;

        if (joueur.equipes != null)
        {
            foreach (STATE_EQUIPE equipe in joueur.equipes)
            {
                if (equipe == null || equipe.membresActuels == null)
                    continue;

                equipe.membresActuels.Remove(personnage);
            }
        }

        joueur.personnagesRecrutes.Remove(personnage);

        Debug.Log(
            $"[FAILLITE] Personnage supprimé | joueur={joueur.nomJoueur} | " +
            $"personnage={personnage.nom} {personnage.prenom}"
        );
    }

    private void InterrompreEquipesVides(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            bool equipeVide =
                equipe.membresActuels == null ||
                !equipe.membresActuels.Exists(p => p != null);

            if (!equipeVide)
                continue;

            if (equipe.AUneActionEnCours || equipe.actionTerminee)
            {
                Debug.Log(
                    $"[FAILLITE] Action interrompue pour équipe vide | " +
                    $"joueur={joueur.nomJoueur} | équipe={equipe.data?.nomEquipe}"
                );
            }

            equipe.actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
            equipe.actionTerminee = false;
            equipe.actionToursRestants = 0;
            equipe.actionToursTotaux = 0;
            equipe.resultatExploration = null;
            equipe.resultatVadrouille = null;
            equipe.provinceAffectee = null;
            equipe.lancementActionAutomatique = false;
            equipe.lancementExplorationAutomatique = false;
        }
    }

    private bool EstPersonnageDansUneEquipeDuJoueur(DATA_JOUEUR joueur, SCOBJ_Personnage personnage)
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

    private void RecalculerRevenuParTourPourJoueur(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return;

        float revenus = 0f;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float totalInfluence =
                province.influenceMaizin +
                province.influenceKinia +
                province.influenceJoho +
                province.influenceAutre;

            if (totalInfluence <= 0f)
                continue;

            float part = 0f;

            switch (joueur.compagnie)
            {
                case ENUM_Compagnie.Maizin:
                    part = province.influenceMaizin / totalInfluence;
                    break;
                case ENUM_Compagnie.Kinia:
                    part = province.influenceKinia / totalInfluence;
                    break;
                case ENUM_Compagnie.Joho:
                    part = province.influenceJoho / totalInfluence;
                    break;
            }

            revenus += province.data.etrinium * part;
        }

        int depenses = 0;

        if (joueur.personnagesRecrutes != null)
        {
            foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
            {
                if (personnage == null)
                    continue;

                bool enExploration = EstPersonnageEnExploration(gameManager, joueur, personnage);
                depenses += enExploration
                    ? SVC_PERSONNAGE_CostService.GetCoutExploration(personnage)
                    : SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);
            }
        }

        if (joueur.equipes != null)
        {
            foreach (STATE_EQUIPE equipe in joueur.equipes)
            {
                if (equipe == null)
                    continue;

                bool aDesMembres =
                    equipe.membresActuels != null &&
                    equipe.membresActuels.Exists(p => p != null);

                int coutEquipe = aDesMembres
                    ? gameManager.CoutFixeEquipeAvecMembresParTour
                    : gameManager.CoutFixeEquipeParTour;

                if (equipe.actionEnCours == ENUM_EQUIPE_ACTION.Exploration)
                {
                    coutEquipe += gameManager.SurcoutEquipeEnExplorationParTour;
                }

                depenses += coutEquipe;
            }
        }

        joueur.etriniumParTour = revenus - depenses;
    }

    private bool EstPersonnageEnExploration(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage)
    {
        if (gameManager == null || joueur == null || personnage == null)
            return false;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || equipe.actionEnCours != ENUM_EQUIPE_ACTION.Exploration || equipe.compagnie != joueur.compagnie)
                continue;

            if (equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Contains(personnage))
                return true;
        }

        return false;
    }
}