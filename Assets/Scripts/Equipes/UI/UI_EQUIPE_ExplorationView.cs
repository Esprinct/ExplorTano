using TMPro;
using UnityEngine;

public class UI_EQUIPE_ExplorationView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject rowExplorationView;

    [Header("UI Exploration")]
    [SerializeField] private TMP_Text toursEnCoursText;
    [SerializeField] private TMP_Text prestigeGagneText;
    [SerializeField] private TMP_Text etriniumParTourText;
    [SerializeField] private TMP_Text toursProjectionText;
    [SerializeField] private TMP_Text chanceArtefactText;
    [SerializeField] private TMP_Text chanceArtefactRareText;
    [SerializeField] private TMP_Text changementInfluenceText;

    public void Refresh(STATE_EQUIPE equipe, SYS_GameManager gameManager)
    {
        bool provinceAffectee =
            equipe != null &&
            equipe.provinceAffectee != null &&
            equipe.provinceAffectee.data != null;

        if (rowExplorationView != null)
            rowExplorationView.SetActive(provinceAffectee);

        if (!provinceAffectee || gameManager == null || gameManager.ExplorationConfig == null)
        {
            Clear();
            return;
        }

        ExplorationPreviewData preview = BuildPreview(equipe, gameManager);
        if (preview == null)
        {
            Clear();
            return;
        }

        bool actionEnCours = equipe.AUneActionEnCours;

        string nomAction = "Action";
        switch (equipe.actionEnCours)
        {
            case ENUM_EQUIPE_ACTION.Vadrouille:
                nomAction = "Vadrouille";
                break;
            case ENUM_EQUIPE_ACTION.Construction:
                nomAction = "Construction";
                break;
            case ENUM_EQUIPE_ACTION.Exploration:
                nomAction = "Exploration";
                break;
        }

        if (toursEnCoursText != null)
        {
            toursEnCoursText.text = actionEnCours
                ? $"{nomAction} : {equipe.actionToursRestants}"
                : "-";
        }

        if (prestigeGagneText != null)
            prestigeGagneText.text = $"+{preview.prestigeFinal} Prestige";

        if (etriniumParTourText != null)
        {
            if (preview.gainEtriniumParTour > 0f)
                etriniumParTourText.text = $"+{preview.gainEtriniumParTour:0.#} etrinium/tour";
            else if (preview.gainEtriniumParTour < 0f)
                etriniumParTourText.text = $"{preview.gainEtriniumParTour:0.#} etrinium/tour";
            else
                etriniumParTourText.text = "+0 etrinium/tour";
        }

        if (toursProjectionText != null)
            toursProjectionText.text = $"Durée : {preview.toursFinaux} tours";

        if (chanceArtefactText != null)
        {
            chanceArtefactText.text =
                $"Artefact : {preview.chanceRelique:0.#}% de chance d'obtenir un artefact";
        }

        if (chanceArtefactRareText != null)
        {
            chanceArtefactRareText.text =
                $"{preview.chanceReliqueRare:0.#}% de chance d'obtenir un artefact rare";
        }

        if (changementInfluenceText != null)
        {
            changementInfluenceText.text =
                $"Occupation : {preview.influenceActuellePct:0.#}% → {preview.influenceProjeteePct:0.#}%\n" +
                $"Exploration : {preview.explorationActuellePct:0.#}% → {preview.explorationProjeteePct:0.#}%";
        }
    }

    private ExplorationPreviewData BuildPreview(STATE_EQUIPE equipe, SYS_GameManager gameManager)
    {
        if (equipe == null || gameManager == null || gameManager.ExplorationConfig == null)
            return null;

        ExplorationConfig config = gameManager.ExplorationConfig;
        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        float explorationActuelle = 0f;

        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);
            explorationActuelle = equipe.provinceAffectee.GetExploration(equipe.compagnie);
        }

        int toursModifies = SVC_EQUIPE_ExplorationEffects.GetToursBaseModifies(
            equipe,
            joueur,
            config.toursBase
        );

        float chanceArtefactModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactModifiee(
            equipe,
            joueur,
            config.chanceArtefactBase
        );

        float chanceArtefactRareModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactRareModifiee(
            equipe,
            joueur,
            config.chanceArtefactRareBase
        );

        float gainExploration = SVC_EQUIPE_ExplorationEffects.GetGainExplorationFinal(
            equipe,
            joueur,
            config.gainExplorationBase
        );

        float explorationProjetee = Mathf.Clamp(explorationActuelle + gainExploration, 0f, 100f);

        ENUM_EXPLORATION_Resultat result = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            toursModifies,
            config.coutParTourBase,
            config.prestigeBase,
            chanceArtefactModifiee,
            chanceArtefactRareModifiee,
            enclavement
        );

        if (result == null)
            return null;

        return new ExplorationPreviewData
        {
            toursFinaux = result.toursFinaux,
            prestigeFinal = result.prestigeFinal,
            chanceRelique = result.chanceRelique,
            chanceReliqueRare = result.chanceReliqueRare,
            influenceActuellePct = CalculerPourcentageInfluenceJoueurDansProvince(equipe),
            influenceProjeteePct = CalculerPourcentageInfluenceJoueurDansProvince(equipe),
            gainEtriniumParTour = 0f,
            explorationActuellePct = explorationActuelle,
            explorationProjeteePct = explorationProjetee
        };
    }

    private void Clear()
    {
        if (toursEnCoursText != null) toursEnCoursText.text = "-";
        if (prestigeGagneText != null) prestigeGagneText.text = "-";
        if (etriniumParTourText != null) etriniumParTourText.text = "-";
        if (toursProjectionText != null) toursProjectionText.text = "-";
        if (chanceArtefactText != null) chanceArtefactText.text = "-";
        if (chanceArtefactRareText != null) chanceArtefactRareText.text = "-";
        if (changementInfluenceText != null) changementInfluenceText.text = "-";
    }

    private float CalculerPourcentageInfluenceJoueurDansProvince(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.provinceAffectee == null)
            return 0f;

        STATE_PROVINCE province = equipe.provinceAffectee;

        float totalInfluence =
            province.influenceMaizin +
            province.influenceKinia +
            province.influenceJoho +
            province.influenceAutre;

        if (totalInfluence <= 0f)
            return 0f;

        float influenceJoueur = 0f;

        switch (equipe.compagnie)
        {
            case ENUM_Compagnie.Maizin:
                influenceJoueur = province.influenceMaizin;
                break;

            case ENUM_Compagnie.Kinia:
                influenceJoueur = province.influenceKinia;
                break;

            case ENUM_Compagnie.Joho:
                influenceJoueur = province.influenceJoho;
                break;
        }

        return (influenceJoueur / totalInfluence) * 100f;
    }

    private class ExplorationPreviewData
    {
        public int toursFinaux;
        public int prestigeFinal;
        public float chanceRelique;
        public float chanceReliqueRare;
        public float influenceActuellePct;
        public float influenceProjeteePct;
        public float explorationActuellePct;
        public float explorationProjeteePct;
        public float gainEtriniumParTour;
    }
}