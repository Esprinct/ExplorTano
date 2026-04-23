using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EQUIPE_HeaderView : MonoBehaviour
{
    [Header("UI Equipe")]
    [SerializeField] private Image portraitChef;
    [SerializeField] private TMP_Text nomEquipe;
    [SerializeField] private TMP_Text nomProvince;
    [SerializeField] private TMP_Text niveau;
    [SerializeField] private TMP_Text statutExploration;
    [SerializeField] private TMP_Text toursRestants;

    [Header("Couleurs statut action")]
    [SerializeField] private Color couleurEnAttenteAffectation = new Color(1f, 0.4f, 0.7f, 1f);
    [SerializeField] private Color couleurAffectee = Color.white;
    [SerializeField] private Color couleurActionEnCours = Color.yellow;
    [SerializeField] private Color couleurActionTerminee = Color.green;

    public void Refresh(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.data == null)
            return;

        if (portraitChef != null)
        {
            portraitChef.sprite = equipe.data.portraitChef;
            portraitChef.enabled = equipe.data.portraitChef != null;
        }

        if (nomEquipe != null)
        {
            nomEquipe.text = equipe.data.nomEquipe;
        }

        if (nomProvince != null)
        {
            nomProvince.text = equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
                ? equipe.provinceAffectee.data.nom
                : "Aucune province";
        }

        if (niveau != null)
        {
            niveau.text = $"Lv. : {equipe.niveauActuel}";
        }

        bool explorationEnCours = equipe.explorationEnCours;
        bool vadrouilleEnCours = equipe.vadrouilleEnCours;
        bool actionEnCours = explorationEnCours || vadrouilleEnCours;

        string labelAction = "Action";
        if (vadrouilleEnCours)
            labelAction = "Vadrouille";
        else if (explorationEnCours)
            labelAction = "Exploration";

        if (statutExploration != null)
        {
            if (actionEnCours)
            {
                statutExploration.text = $"{labelAction} en cours";
                statutExploration.color = couleurActionEnCours;
            }
            else if (equipe.actionTerminee)
            {
                statutExploration.text = "Action terminée";
                statutExploration.color = couleurActionTerminee;
            }
            else if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
            {
                statutExploration.text = "Affectée";
                statutExploration.color = couleurAffectee;
            }
            else
            {
                statutExploration.text = "En attente d'affectation";
                statutExploration.color = couleurEnAttenteAffectation;
            }
        }

        if (toursRestants != null)
        {
            if (actionEnCours && equipe.toursTotaux > 0)
            {
                toursRestants.text = $"Tours restants : {equipe.toursRestants} / {equipe.toursTotaux}";
            }
            else
            {
                toursRestants.text = "Aucune action en cours";
            }
        }
    }
}