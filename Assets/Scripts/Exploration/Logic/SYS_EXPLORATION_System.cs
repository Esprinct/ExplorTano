using System.Collections.Generic;
using UnityEngine;

public class ExplorationSystem
{
    private readonly SYS_InfluenceSystem influenceSystem;
    private readonly SYS_GameUiRefreshService uiSystem;

    public ExplorationSystem(SYS_InfluenceSystem influenceSystem, SYS_GameUiRefreshService uiSystem)
    {
        this.influenceSystem = influenceSystem;
        this.uiSystem = uiSystem;
    }

    public void DemarrerExploration(SYS_GameManager gameManager, STATE_EQUIPE equipe, ENUM_Compagnie compagnie, int dureeTours)
    {
        if (equipe == null || gameManager == null)
            return;

        equipe.compagnie = compagnie;
        equipe.explorationTerminee = false;

        ExplorationConfig config = gameManager.ExplorationConfig;

        int toursBase = config != null ? config.toursBase : dureeTours;
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

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        if (joueur == null)
            return;

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

        equipe.explorationEnCours = true;
        equipe.toursRestants = result.toursFinaux;
        equipe.toursTotaux = result.toursFinaux;
        equipe.resultatExploration = result;

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem.RefreshToutLeHUD(gameManager);

        Debug.Log(
            $"[EXPLORATION START] Equipe={equipe.data?.nomEquipe} | " +
            $"Province={equipe.provinceAffectee?.data?.nom} | " +
            $"Tours base={toursBase} | Tours finaux={result.toursFinaux} | " +
            $"Chance artefact={result.chanceRelique:0.##}% | " +
            $"Chance artefact rare={result.chanceReliqueRare:0.##}% | " +
            $"Prestige attendu={result.prestigeFinal} | " +
            $"Coût lancement={coutLancement}"
        );
    }

    public void MettreAJourExplorations(SYS_GameManager gameManager)
    {
        if (gameManager == null || gameManager.EquipesRuntime == null)
            return;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || !equipe.explorationEnCours)
                continue;

            equipe.toursRestants--;

            if (equipe.toursRestants <= 0)
            {
                TerminerExploration(gameManager, equipe);
            }
        }
    }

    private void DonnerXpEquipe(STATE_EQUIPE equipe, ExplorationConfig config)
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

    private void DonnerXpEquipeRuntime(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return;

        if (equipe.progression == null)
            equipe.progression = new STATE_LevelProgression();

        if (equipe.progressionConfig == null)
        {
            SYS_GameManager gm = Object.FindAnyObjectByType<SYS_GameManager>();
            if (gm != null)
                equipe.progressionConfig = gm.ProgressionConfigEquipe;
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
        if (config == null)
            return null;

        ENUM_EXPLORATION_Resultat resultat = equipe.resultatExploration;
        if (resultat == null)
            return null;

        float chanceRare = Mathf.Clamp(resultat.chanceReliqueRare, 0f, 100f);
        float chanceNormale = Mathf.Clamp(resultat.chanceRelique, 0f, 100f);

        float rollRare = Random.Range(0f, 100f);
        if (rollRare <= chanceRare)
        {
            SCOBJ_OBJET_EQUIPPABLE artefactRare = TirerArtefactDepuisPool(config, true);

            if (artefactRare != null)
            {
                UTIL_JOUEUR_INVENTAIRE.AjouterObjetAuJoueur(joueur, artefactRare, 1);

                Debug.Log(
                    $"[ARTEFACT] Artefact rare obtenu | " +
                    $"joueur={joueur.nomJoueur} | " +
                    $"equipe={equipe.data?.nomEquipe} | " +
                    $"objet={artefactRare.nom} | " +
                    $"rollRare={rollRare:0.00} | chanceRare={chanceRare:0.00}"
                );

                return artefactRare;
            }

            Debug.LogWarning(
                $"[ARTEFACT] Jet rare réussi mais aucun artefact rare disponible | " +
                $"rollRare={rollRare:0.00} | chanceRare={chanceRare:0.00}"
            );
        }

        float rollNormal = Random.Range(0f, 100f);
        if (rollNormal <= chanceNormale)
        {
            SCOBJ_OBJET_EQUIPPABLE artefactNormal = TirerArtefactDepuisPool(config, false);

            if (artefactNormal != null)
            {
                UTIL_JOUEUR_INVENTAIRE.AjouterObjetAuJoueur(joueur, artefactNormal, 1);

                Debug.Log(
                    $"[ARTEFACT] Artefact normal obtenu | " +
                    $"joueur={joueur.nomJoueur} | " +
                    $"equipe={equipe.data?.nomEquipe} | " +
                    $"objet={artefactNormal.nom} | " +
                    $"rollNormal={rollNormal:0.00} | chanceNormale={chanceNormale:0.00}"
                );

                return artefactNormal;
            }

            Debug.LogWarning(
                $"[ARTEFACT] Jet normal réussi mais aucun artefact commun disponible | " +
                $"rollNormal={rollNormal:0.00} | chanceNormale={chanceNormale:0.00}"
            );
        }

        Debug.Log(
            $"[ARTEFACT] Aucun artefact obtenu | " +
            $"chanceRare={chanceRare:0.00} | chanceNormale={chanceNormale:0.00}"
        );

        return null;
    }

    private SCOBJ_OBJET_EQUIPPABLE TirerArtefactDepuisPool(ExplorationConfig config, bool rare)
    {
        if (config == null)
            return null;

        List<SCOBJ_OBJET_EQUIPPABLE> pool = rare
            ? config.artefactsRares
            : config.artefactsCommuns;

        if (PoolEstVide(pool))
        {
            if (rare && !PoolEstVide(config.artefactsCommuns))
            {
                pool = config.artefactsCommuns;
            }
            else
            {
                return null;
            }
        }

        int index = Random.Range(0, pool.Count);
        return pool[index];
    }

    private static bool PoolEstVide(List<SCOBJ_OBJET_EQUIPPABLE> pool)
    {
        return pool == null || pool.Count == 0;
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
        {
            Debug.LogWarning("[POPUP] Aucun humain trouvé.");
            return;
        }

        if (humain.equipes == null || !humain.equipes.Contains(equipe))
        {
            Debug.LogWarning("[POPUP] équipe non humaine, popup annulée");
            return;
        }

        UI_EXPLORATION_RecompensePopup popup = gameManager.ExplorationRecompensePopup;

        if (popup == null)
        {
            Debug.LogWarning("[POPUP] Référence popup null sur GameManager, recherche auto...");
            popup = Object.FindAnyObjectByType<UI_EXPLORATION_RecompensePopup>(FindObjectsInactive.Include);
            gameManager.ExplorationRecompensePopup = popup;
        }

        if (popup == null)
        {
            Debug.LogError("[POPUP] UI_EXPLORATION_RecompensePopup introuvable.");
            return;
        }

        if (humain.equipes == null || !humain.equipes.Contains(equipe))
        {
            Debug.Log(
                $"[POPUP] ignorée pour équipe non humaine | " +
                $"équipe={equipe?.data?.nomEquipe} | compagnie={equipe?.compagnie}"
            );
            return;
        }

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

    private void TerminerExploration(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (equipe == null || gameManager == null)
            return;

        STATE_PROVINCE province = equipe.provinceAffectee;
        DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(equipe.compagnie);

        int gainPrestige = 1;

        if (equipe.resultatExploration != null)
        {
            gainPrestige = equipe.resultatExploration.prestigeFinal;
        }

        // V2 : l'exploration augmente uniquement le % d'exploration
        if (province != null && joueur != null)
        {
            float gainExploration = CalculerGainExploration(gameManager, equipe, joueur);
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

        SCOBJ_OBJET_EQUIPPABLE artefactGagne = null;

        if (joueur != null)
        {
            joueur.prestige += gainPrestige;
            DonnerXpEquipe(equipe, gameManager.ExplorationConfig);
            DonnerXpEquipeRuntime(equipe);
            artefactGagne = DonnerArtefactFinExploration(gameManager, joueur, equipe);
        }

        equipe.explorationEnCours = false;
        equipe.explorationTerminee = true;
        equipe.toursRestants = 0;
        equipe.toursTotaux = 0;

        if (!equipe.affectationAutomatique)
        {
            equipe.provinceAffectee = null;
        }

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem.RefreshToutLeHUD(gameManager);

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();

        bool equipeHumaine =
            humain != null &&
            humain.equipes != null &&
            humain.equipes.Contains(equipe);

        if (equipeHumaine)
        {
            AfficherPopupRecompenseExploration(
                gameManager,
                equipe,
                province,
                gainPrestige,
                artefactGagne
            );
        }

        if (equipe.lancementExplorationAutomatique &&
            equipe.provinceAffectee != null &&
            equipe.provinceAffectee.data != null)
        {
            DemarrerExploration(gameManager, equipe, equipe.compagnie, 0);
            return;
        }

        Debug.Log($"Exploration terminée pour {equipe.data?.nomEquipe}");
    }}

    private float CalculerGainExploration(SYS_GameManager gameManager, STATE_EQUIPE equipe, DATA_JOUEUR joueur)
    {
        if (gameManager == null || gameManager.ExplorationConfig == null)
            return 0f;

        float baseGain = gameManager.ExplorationConfig.gainExplorationBase;

        float gainFinal = SVC_EQUIPE_ExplorationEffects.GetGainExplorationFinal(
            equipe,
            joueur,
            baseGain
        );

        return Mathf.Max(0f, gainFinal);
    }
}