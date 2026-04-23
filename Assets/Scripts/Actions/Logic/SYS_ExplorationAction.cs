using System.Collections.Generic;
using UnityEngine;

public class SYS_ExplorationAction : SYS_EquipeActionBase
{
    private readonly SYS_InfluenceSystem influenceSystem;

    public SYS_ExplorationAction(
        SYS_InfluenceSystem influenceSystem,
        SYS_GameUiRefreshService uiSystem) : base(uiSystem)
    {
        this.influenceSystem = influenceSystem;
    }

    public override ENUM_EQUIPE_ACTION TypeAction => ENUM_EQUIPE_ACTION.Exploration;

    public override void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (equipe == null || gameManager == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        if (joueur == null)
            return;

        equipe.compagnie = joueur.compagnie;
        equipe.explorationTerminee = false;

        ExplorationConfig config = gameManager.ExplorationConfig;
        int toursBase = config != null ? config.toursBase : 3;
        int coutParTourBase = config != null ? config.coutParTourBase : 5;
        int prestigeBase = config != null ? config.prestigeBase : 1;
        float chanceArtefactBase = config != null ? config.chanceArtefactBase : 10f;
        float chanceArtefactRareBase = config != null ? config.chanceArtefactRareBase : 2f;

        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);
        }

        int toursModifies = SVC_EQUIPE_ExplorationEffects.GetToursBaseModifies(
            equipe,
            joueur,
            toursBase
        );

        float chanceArtefactModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactModifiee(
            equipe,
            joueur,
            chanceArtefactBase
        );

        float chanceArtefactRareModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactRareModifiee(
            equipe,
            joueur,
            chanceArtefactRareBase
        );

        ENUM_EXPLORATION_Resultat result = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            toursModifies,
            coutParTourBase,
            prestigeBase,
            chanceArtefactModifiee,
            chanceArtefactRareModifiee,
            enclavement
        );

        if (result == null)
            return;

        int coutLancement = result.coutTotal;

        if (joueur.etrinium < coutLancement)
        {
            Debug.LogWarning(
                $"Pas assez d'étrinium pour lancer l'exploration. " +
                $"Disponible={joueur.etrinium} | Requis={coutLancement}"
            );
            return;
        }

        joueur.etrinium -= coutLancement;

        equipe.resultatExploration = result;
        InitialiserAction(equipe, TypeAction, result.toursFinaux);

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);

        Debug.Log(
            $"[EXPLORATION_ACTION_START] Equipe={equipe.data?.nomEquipe} | " +
            $"Province={equipe.provinceAffectee?.data?.nom} | " +
            $"Tours finaux={result.toursFinaux} | " +
            $"Chance artefact={result.chanceRelique:0.##}% | " +
            $"Chance artefact rare={result.chanceReliqueRare:0.##}% | " +
            $"Prestige attendu={result.prestigeFinal} | " +
            $"Coût lancement={coutLancement}"
        );
    }

    public override void MettreAJour(SYS_GameManager gameManager)
    {
        if (gameManager == null || gameManager.EquipesRuntime == null)
            return;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (!PeutTraiter(equipe))
                continue;

            equipe.actionToursRestants--;

            if (equipe.actionToursRestants <= 0)
            {
                Terminer(gameManager, equipe);
            }
        }
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (equipe == null || gameManager == null)
            return;

        STATE_PROVINCE province = equipe.provinceAffectee;
        DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(equipe.compagnie);

        if (province == null || joueur == null)
        {
            CloturerAction(equipe);
            return;
        }

        ExplorationConfig config = gameManager.ExplorationConfig;

if (config != null)
{
    float gainExploration = SVC_EQUIPE_ExplorationEffects.GetGainExplorationFinal(
        equipe,
        joueur,
        config.gainExplorationBase
    );

    float avant = province.GetExploration(equipe.compagnie);
    province.AjouterExploration(equipe.compagnie, gainExploration);
    float apres = province.GetExploration(equipe.compagnie);

    Debug.Log(
        $"[EXPLORATION_PCT] province={province.data?.nom} | " +
        $"compagnie={equipe.compagnie} | " +
        $"equipe={equipe.data?.nomEquipe} | " +
        $"gain=+{gainExploration:0.##}% | " +
        $"avant={avant:0.##}% | après={apres:0.##}%"
    );

    if (province.EstEntierementExploreePar(equipe.compagnie))
    {
        Debug.Log(
            $"[EXPLORATION_PCT] Province entièrement explorée par {equipe.compagnie} : {province.data?.nom}"
        );
    }

    joueur.prestige += equipe.resultatExploration != null
        ? equipe.resultatExploration.prestigeFinal
        : config.prestigeBase;

    DonnerXpPersonnages(equipe, config);
    DonnerXpEquipeRuntime(equipe, gameManager);

    SCOBJ_OBJET_EQUIPPABLE artefactGagne = DonnerArtefactFinExploration(
        gameManager,
        joueur,
        equipe
    );

    AfficherPopupRecompenseExploration(
        gameManager,
        equipe,
        province,
        equipe.resultatExploration != null ? equipe.resultatExploration.prestigeFinal : 0,
        artefactGagne
    );
}

        influenceSystem.MettreAJourClaimProvince(gameManager, province);

        CloturerAction(equipe);
        equipe.explorationTerminee = true;
        equipe.resultatExploration = null;

        if (!equipe.affectationAutomatique)
        {
            equipe.provinceAffectee = null;
        }

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);

        if (equipe.lancementExplorationAutomatique &&
            equipe.provinceAffectee != null &&
            equipe.provinceAffectee.data != null)
        {
            Demarrer(gameManager, equipe);
            return;
        }

        Debug.Log($"[EXPLORATION_ACTION_END] Equipe={equipe.data?.nomEquipe}");
    }

    private void DonnerXpPersonnages(STATE_EQUIPE equipe, ExplorationConfig config)
    {
        if (equipe == null || equipe.membresActuels == null)
            return;

        int xp = config != null ? config.xpPersonnageParExploration : 25;

        foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
        {
            if (personnage == null)
                continue;

            if (personnage.progression == null || personnage.progressionConfig == null)
            {
                Debug.LogWarning($"XP non appliquée (progression manquante) : {personnage.nom} {personnage.prenom}");
                continue;
            }

            int niveauxGagnes = SVC_LevelProgression.AddXp(
                personnage.progression,
                personnage.progressionConfig,
                xp
            );

            Debug.Log(
                $"[XP] {personnage.nom} {personnage.prenom} +{xp} XP | " +
                $"Niveau={personnage.progression.niveau} | +{niveauxGagnes} niveaux"
            );
        }
    }

    private void DonnerXpEquipeRuntime(STATE_EQUIPE equipe, SYS_GameManager gameManager)
    {
        if (equipe == null)
            return;

        if (equipe.progression == null)
            equipe.progression = new STATE_LevelProgression();

        if (equipe.progressionConfig == null)
        {
            if (gameManager != null)
                equipe.progressionConfig = gameManager.ProgressionConfigEquipe;
        }

        if (equipe.progression == null || equipe.progressionConfig == null)
        {
            Debug.LogWarning(
                $"[XP_EQUIPE] Progression manquante pour {equipe.data?.nomEquipe} | " +
                $"progressionNull={equipe.progression == null} | " +
                $"progressionConfigNull={equipe.progressionConfig == null}"
            );
            return;
        }

        int xpEquipe = 50;
        int ancienNiveau = equipe.progression.niveau;

        int niveauxGagnes = SVC_LevelProgression.AddXp(
            equipe.progression,
            equipe.progressionConfig,
            xpEquipe
        );

        equipe.SynchroniserNiveauLegacyDepuisProgression();

        Debug.Log(
            $"[XP_EQUIPE] {equipe.data?.nomEquipe} +{xpEquipe} XP | " +
            $"Niveau={equipe.progression.niveau} | +{niveauxGagnes} niveaux"
        );

        if (ancienNiveau < 3 &&
            equipe.NiveauActuel >= 3 &&
            equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
        {
            Debug.Log($"[SPECIALISATION] {equipe.data?.nomEquipe} a débloqué le choix Tier 2.");
        }

        if (ancienNiveau < 6 &&
            equipe.NiveauActuel >= 6 &&
            (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Exploration ||
             equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Construction ||
             equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Miliciens))
        {
            Debug.Log($"[SPECIALISATION] {equipe.data?.nomEquipe} a débloqué le choix Tier 3.");
        }
    }

    private SCOBJ_OBJET_EQUIPPABLE DonnerArtefactFinExploration(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        STATE_EQUIPE equipe)
    {
        if (gameManager == null || joueur == null || equipe == null)
            return null;

        ExplorationConfig config = gameManager.ExplorationConfig;
        ENUM_EXPLORATION_Resultat resultat = equipe.resultatExploration;

        if (config == null || resultat == null)
            return null;

        float rollRare = Random.Range(0f, 100f);
        float rollCommun = Random.Range(0f, 100f);

        SCOBJ_OBJET_EQUIPPABLE artefact = null;

        if (rollRare <= resultat.chanceReliqueRare)
        {
            artefact = TirerArtefactDepuisPool(config.artefactsRares);
        }
        else if (rollCommun <= resultat.chanceRelique)
        {
            artefact = TirerArtefactDepuisPool(config.artefactsCommuns);
        }

        if (artefact == null)
            return null;

        joueur.objetsPossedes ??= new List<SCOBJ_OBJET>();
        joueur.objetsPossedes.Add(artefact);

        Debug.Log($"[ARTEFACT] {artefact.nom} attribué à {joueur.nomJoueur}");
        return artefact;
    }

    private SCOBJ_OBJET_EQUIPPABLE TirerArtefactDepuisPool(List<SCOBJ_OBJET_EQUIPPABLE> pool)
    {
        if (pool == null || pool.Count == 0)
            return null;

        int index = Random.Range(0, pool.Count);
        return pool[index];
    }

    private void AfficherPopupRecompenseExploration(
        SYS_GameManager gameManager,
        STATE_EQUIPE equipe,
        STATE_PROVINCE province,
        int gainPrestige,
        SCOBJ_OBJET_EQUIPPABLE artefactGagne)
    {
        if (gameManager == null || equipe == null)
            return;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return;

        if (humain.equipes == null || !humain.equipes.Contains(equipe))
            return;

        UI_EXPLORATION_RecompensePopup popup = gameManager.ExplorationRecompensePopup;

        if (popup == null)
        {
            popup = Object.FindAnyObjectByType<UI_EXPLORATION_RecompensePopup>(FindObjectsInactive.Include);
            gameManager.ExplorationRecompensePopup = popup;
        }

        if (popup == null)
            return;

        ExplorationConfig config = gameManager.ExplorationConfig;

        DATA_EXPLORATION_RecompensePopup data = new DATA_EXPLORATION_RecompensePopup
        {
            nomEquipe = equipe.data != null ? equipe.data.nomEquipe : "Équipe",
            nomProvince = province != null && province.data != null ? province.data.nom : "Province inconnue",
            prestigeGagne = gainPrestige,
            xpGagneParPersonnage = config != null ? config.xpPersonnageParExploration : 25,
            artefactTrouve = artefactGagne != null,
            nomArtefact = artefactGagne != null ? artefactGagne.nom : "",
            descriptionArtefact = artefactGagne != null ? artefactGagne.description : "",
            iconeArtefact = artefactGagne != null ? artefactGagne.icone : null,
            rareteArtefact = artefactGagne != null ? artefactGagne.rareteEtoiles : 0
        };

        popup.OpenMenu(data, true);
    }
}