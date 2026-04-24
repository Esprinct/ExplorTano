using System.Collections.Generic;
using UnityEngine;

public class SYS_DebugEquipesRuntimeView : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private SYS_GameManager gameManager;

    [Header("Debug Runtime Adversaires")]
    [SerializeField] private List<DATA_DebugEquipeRuntimeView> debugEquipesJoueur2 = new();
    [SerializeField] private List<DATA_DebugEquipeRuntimeView> debugEquipesJoueur3 = new();
    [SerializeField] private bool refreshDebugEquipesChaqueFrame = false;

    public IReadOnlyList<DATA_DebugEquipeRuntimeView> DebugEquipesJoueur2 => debugEquipesJoueur2;
    public IReadOnlyList<DATA_DebugEquipeRuntimeView> DebugEquipesJoueur3 => debugEquipesJoueur3;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GetComponent<SYS_GameManager>();

        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();
    }

    private void LateUpdate()
    {
        if (refreshDebugEquipesChaqueFrame)
            RefreshDebugEquipesAdverses();
    }

    public void RefreshDebugEquipesAdverses()
    {
        if (gameManager == null)
            return;

        debugEquipesJoueur2 = BuildDebugEquipesForPlayer(gameManager.Joueur2);
        debugEquipesJoueur3 = BuildDebugEquipesForPlayer(gameManager.Joueur3);
    }

    private List<DATA_DebugEquipeRuntimeView> BuildDebugEquipesForPlayer(DATA_JOUEUR joueur)
    {
        List<DATA_DebugEquipeRuntimeView> result = new();

        if (joueur == null || joueur.equipes == null)
            return result;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            DATA_DebugEquipeRuntimeView debugEquipe = new DATA_DebugEquipeRuntimeView
            {
                nomEquipe = equipe.data != null ? equipe.data.nomEquipe : "Equipe",
                compagnie = equipe.compagnie,
                province = equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
                    ? equipe.provinceAffectee.data.nom
                    : "Aucune",
                actionEnCours = equipe.actionEnCours,
                actionTerminee = equipe.actionTerminee,
                actionToursRestants = equipe.actionToursRestants,
                actionToursTotaux = equipe.actionToursTotaux,
                affectationAutomatique = equipe.affectationAutomatique,
                lancementActionAutomatique = equipe.lancementActionAutomatique,
                niveauEquipe = equipe.niveauActuel,
                membres = new List<DATA_DebugPersonnageRuntimeView>()
            };

            if (equipe.membresActuels != null)
            {
                foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
                {
                    if (personnage == null)
                        continue;

                    int niveau = personnage.progression != null
                        ? personnage.progression.niveau
                        : 1;

                    debugEquipe.membres.Add(new DATA_DebugPersonnageRuntimeView
                    {
                        nomComplet = $"{personnage.prenom} {personnage.nom}".Trim(),
                        role = personnage.roleActuel.ToString(),
                        rarete = personnage.rareteEtoiles,
                        niveau = niveau,
                        force = personnage.force,
                        intelligence = personnage.intelligence,
                        dexterite = personnage.dexterite,
                        endurance = personnage.endurance,
                        coutParTour = personnage.coutParTour
                    });
                }
            }

            RemplirStatsDebugEquipe(debugEquipe, equipe);
            result.Add(debugEquipe);
        }

        return result;
    }

    private void RemplirStatsDebugEquipe(DATA_DebugEquipeRuntimeView debugEquipe, STATE_EQUIPE equipe)
    {
        if (debugEquipe == null || equipe == null || gameManager == null)
            return;

        debugEquipe.nombreMembres = 0;
        debugEquipe.forceTotale = 0;
        debugEquipe.intelligenceTotale = 0;
        debugEquipe.dexteriteTotale = 0;
        debugEquipe.enduranceTotale = 0;
        debugEquipe.coutPersonnagesParTour = 0;

        if (equipe.membresActuels != null)
        {
            foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
            {
                if (personnage == null)
                    continue;

                debugEquipe.nombreMembres++;
                debugEquipe.forceTotale += personnage.force;
                debugEquipe.intelligenceTotale += personnage.intelligence;
                debugEquipe.dexteriteTotale += personnage.dexterite;
                debugEquipe.enduranceTotale += personnage.endurance;
                debugEquipe.coutPersonnagesParTour += personnage.coutParTour;
            }
        }

        bool aDesMembres = debugEquipe.nombreMembres > 0;

        debugEquipe.coutFixeEquipeParTourEstime = aDesMembres
            ? gameManager.CoutFixeEquipeAvecMembresParTour
            : gameManager.CoutFixeEquipeParTour;

        if (equipe.actionEnCours == ENUM_EQUIPE_ACTION.Exploration)
        {
            debugEquipe.coutFixeEquipeParTourEstime += gameManager.SurcoutEquipeEnExplorationParTour;
        }

        debugEquipe.coutTotalEquipeParTourEstime =
            debugEquipe.coutPersonnagesParTour +
            debugEquipe.coutFixeEquipeParTourEstime;
    }
}