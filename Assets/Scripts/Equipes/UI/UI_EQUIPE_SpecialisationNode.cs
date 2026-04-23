using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EQUIPE_SpecialisationNode : MonoBehaviour
{
    [Header("Contenu principal")]
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconeImage;
    [SerializeField] private Button bouton;

    [Header("États visuels")]
    [SerializeField] private GameObject badgeActuel;
    [SerializeField] private GameObject badgeDisponible;
    [SerializeField] private GameObject badgeVerrouille;
    [SerializeField] private GameObject overlayVerrouille;
    [SerializeField] private GameObject highlightSelection;

    [Header("Textes")]
    [SerializeField] private TMP_Text texteCondition;
    [SerializeField] private TMP_Text texteBonus;

    private SCOBJ_EQUIPE_SPECIALISATION specialisation;
    private Action<SCOBJ_EQUIPE_SPECIALISATION> onClickCallback;

    private void Awake()
    {
        if (bouton != null)
            bouton.onClick.AddListener(OnClickNode);

        ClearVisualState();
    }

    private void OnDestroy()
    {
        if (bouton != null)
            bouton.onClick.RemoveListener(OnClickNode);
    }

    public void Setup(
        SCOBJ_EQUIPE_SPECIALISATION data,
        string bonusText,
        string conditionText,
        bool estActuelle,
        bool estDebloquee,
        bool estDisponibleMaintenant,
        bool estSelectionnee,
        Action<SCOBJ_EQUIPE_SPECIALISATION> onClick)
    {
        specialisation = data;
        onClickCallback = onClick;

        if (nomText != null)
            nomText.text = data != null ? data.nomAffiche : "Spécialisation";

        if (descriptionText != null)
            descriptionText.text = data != null ? data.description : "";

        if (iconeImage != null)
        {
            iconeImage.sprite = data != null ? data.icone : null;
            iconeImage.enabled = iconeImage.sprite != null;
        }

        if (texteCondition != null)
            texteCondition.text = conditionText ?? "";

        if (texteBonus != null)
            texteBonus.text = bonusText ?? "";

        RefreshBadges(estActuelle, estDebloquee, estDisponibleMaintenant);
        SetSelected(estSelectionnee);

        // IMPORTANT :
        // on laisse toujours le node cliquable, même verrouillé,
        // pour afficher les infos dans le panneau du bas.
        if (bouton != null)
            bouton.interactable = data != null;
    }

   private void RefreshBadges(bool estActuelle, bool estDebloquee, bool estDisponibleMaintenant)
{
    ClearBadgeState();

    if (estActuelle)
    {
        if (badgeActuel != null)
            badgeActuel.SetActive(true);
        return;
    }

    if (estDisponibleMaintenant)
    {
        if (badgeDisponible != null)
            badgeDisponible.SetActive(true);
        return;
    }

    if (estDebloquee)
    {
        // soit un nouveau badgeDebloque
        // soit aucun check du tout, juste un style plus discret
        return;
    }

    if (badgeVerrouille != null)
        badgeVerrouille.SetActive(true);

    if (overlayVerrouille != null)
        overlayVerrouille.SetActive(true);
}

    private void ClearBadgeState()
    {
        if (badgeActuel != null)
            badgeActuel.SetActive(false);

        if (badgeDisponible != null)
            badgeDisponible.SetActive(false);

        if (badgeVerrouille != null)
            badgeVerrouille.SetActive(false);

        if (overlayVerrouille != null)
            overlayVerrouille.SetActive(false);
    }

    private void ClearVisualState()
    {
        ClearBadgeState();

        if (highlightSelection != null)
            highlightSelection.SetActive(false);

        if (nomText != null)
            nomText.text = "";

        if (descriptionText != null)
            descriptionText.text = "";

        if (texteCondition != null)
            texteCondition.text = "";

        if (texteBonus != null)
            texteBonus.text = "";

        if (iconeImage != null)
        {
            iconeImage.sprite = null;
            iconeImage.enabled = false;
        }
    }

    public SCOBJ_EQUIPE_SPECIALISATION GetSpecialisation()
    {
        return specialisation;
    }

    public void SetSelected(bool selected)
    {
        if (highlightSelection != null)
            highlightSelection.SetActive(selected);
    }

    private void OnClickNode()
    {
        if (specialisation == null)
            return;

        onClickCallback?.Invoke(specialisation);
    }
}