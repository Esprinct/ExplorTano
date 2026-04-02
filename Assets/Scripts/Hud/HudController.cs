using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [Header("HUD Dirigeant")]
    [SerializeField] private Image portraitDirigeantImage;
    [SerializeField] private Image logoCompagnieImage;
    [SerializeField] private TMP_Text nomDirigeantText;
    [SerializeField] private TMP_Text niveauDirigeantText;

    [Header("HUD Ressources")]
    [SerializeField] private TMP_Text etriniumText;
    [SerializeField] private TMP_Text prestigeText;
    [SerializeField] private TMP_Text provincesControleesText;
    [SerializeField] private TMP_Text positionJoueurText;

    [Header("HUD Tour")]
    [SerializeField] private Button boutonTourSuivant;
    [SerializeField] private TMP_Text tourText;

    [Header("HUD Equipes")]
    [SerializeField] private List<HudEquipeSlot> equipeSlots = new();
    [SerializeField] private EquipeMenuController equipeMenuController;

    [Header("Données de test (optionnel)")]
    [SerializeField] private bool utiliserDonneesDeTestAuStart = false;
    [SerializeField] private JoueurHudData joueurTest;
    [SerializeField] private PartieHudData partieTest;
    [SerializeField] private List<EquipeHudData> equipesTest = new();

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        if (boutonTourSuivant != null)
        {
            boutonTourSuivant.onClick.AddListener(OnBoutonTourSuivantClicked);
        }
    }

   private void Start()
{
    foreach (HudEquipeSlot slot in equipeSlots)
    {
        if (slot != null)
        {
            slot.SetMenuController(equipeMenuController);
        }
    }

    if (gameManager == null && utiliserDonneesDeTestAuStart)
    {
        RefreshAll(joueurTest, partieTest, equipesTest);
    }
}

    private void OnDestroy()
    {
        if (boutonTourSuivant != null)
        {
            boutonTourSuivant.onClick.RemoveListener(OnBoutonTourSuivantClicked);
        }
    }

    public void RefreshAll(JoueurHudData joueurData, PartieHudData partieData, List<EquipeHudData> equipesData)
    {
        RefreshDirigeant(joueurData);
        RefreshRessources(joueurData);
        RefreshTour(partieData);
        RefreshEquipes(equipesData);
    }

    public void RefreshDirigeant(JoueurHudData joueurData)
    {
        if (joueurData == null)
        {
            return;
        }

        if (portraitDirigeantImage != null)
        {
            portraitDirigeantImage.sprite = joueurData.portraitDirigeant;
            portraitDirigeantImage.enabled = joueurData.portraitDirigeant != null;
        }

        if (logoCompagnieImage != null)
        {
            logoCompagnieImage.sprite = joueurData.logoCompagnie;
            logoCompagnieImage.enabled = joueurData.logoCompagnie != null;
        }

        if (nomDirigeantText != null)
        {
            nomDirigeantText.text = joueurData.nomDirigeant;
        }

        if (niveauDirigeantText != null)
        {
            niveauDirigeantText.text = $"{joueurData.niveauDirigeant}";
        }
    }

    public void RefreshRessources(JoueurHudData joueurData)
    {
        if (joueurData == null)
        {
            return;
        }

        if (etriniumText != null)
        {
            etriniumText.text = $"{joueurData.etriniumTotal} (+{joueurData.etriniumParTour})";
        }

        if (prestigeText != null)
        {
            prestigeText.text = $"{joueurData.prestige}";
        }

        if (provincesControleesText != null)
        {
            provincesControleesText.text = $"{joueurData.provincesControlees}";
        }

        if (positionJoueurText != null)
        {
            positionJoueurText.text = joueurData.positionTexte;
        }
    }

    public void RefreshTour(PartieHudData partieData)
    {
        if (partieData == null)
        {
            return;
        }

        if (tourText != null)
        {
            tourText.text = $"{partieData.tourActuel} / {partieData.tourMax}";
        }
    }

    public void RefreshEquipes(List<EquipeHudData> equipesData)
    {
        for (int i = 0; i < equipeSlots.Count; i++)
        {
            if (equipesData != null && i < equipesData.Count && equipesData[i] != null)
            {
                equipeSlots[i].Refresh(equipesData[i]);
            }
            else
            {
                equipeSlots[i].Hide();
            }
        }
    }

    public void RefreshFromGameManager()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager introuvable.");
            return;
        }

        gameManager.RefreshToutLeHUD();
    }

    private void OnBoutonTourSuivantClicked()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager introuvable.");
            return;
        }

        gameManager.TourSuivant();
    }
}