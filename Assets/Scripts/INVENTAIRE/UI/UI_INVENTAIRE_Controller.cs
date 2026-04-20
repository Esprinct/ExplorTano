using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_INVENTAIRE_Controller : UTIL_UiPanelControllerBase
{
    [Header("Navigation")]
    [SerializeField] private Button boutonFermer;
    [SerializeField] private Button boutonOngletPersonnages;
    [SerializeField] private Button boutonOngletObjets;

    [Header("Onglets")]
    [SerializeField] private UI_INVENTAIRE_ONGLET_PERSONNAGE personnageOngletView;
    [SerializeField] private UI_INVENTAIRE_ONGLET_OBJET objetOngletView;

    private ENUM_INVENTAIRE_Onglet ongletActif = ENUM_INVENTAIRE_Onglet.Personnages;

    private List<SCOBJ_Personnage> personnagesCourants = new();
    private List<SCOBJ_OBJET> objetsCourants = new();
    private List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommablesCourants = new();

    private UI_EQUIPE_DetailController equipeMenuCible;
    private bool modeAjoutEquipe = false;
    private DATA_PERSONNAGE_DisplayContext contexteAffichage = DATA_PERSONNAGE_DisplayContext.Default;

    private bool modeSelectionPersonnage = false;
    private Action<SCOBJ_Personnage> onPersonnageSelectionne;

    private bool modeSelectionObjetEquipable = false;
    private ENUM_OBJET_EQUIPPABLE typeEquipementSelection = ENUM_OBJET_EQUIPPABLE.Outil;
    private Action<SCOBJ_OBJET_EQUIPPABLE> onObjetEquipableSelectionne;

    private Canvas rootCanvas;
    private int sortingOrderInitial;
    private bool sortingOrderSauvegarde;

    private Transform parentInitial;
    private int siblingIndexInitial;
    private bool ordreInitialSauvegarde;
    private bool estOuvertEnOverlaySelection;

    private void Awake()
    {
        AutoBind();

        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            sortingOrderInitial = rootCanvas.sortingOrder;
            sortingOrderSauvegarde = true;
        }

        UTIL_UiEventBinder.Bind(boutonFermer, CloseMenu, this, nameof(boutonFermer));
        UTIL_UiEventBinder.Bind(boutonOngletPersonnages, OuvrirOngletPersonnages, this, nameof(boutonOngletPersonnages));

        if (boutonOngletObjets != null)
        {
            UTIL_UiEventBinder.Bind(boutonOngletObjets, OuvrirOngletObjets, this, nameof(boutonOngletObjets));
        }

        ClosePanel();
        RefreshOnglets();
    }

    private void OnDestroy()
    {
        UTIL_UiEventBinder.Unbind(boutonFermer, CloseMenu);

        if (boutonOngletPersonnages != null)
            UTIL_UiEventBinder.Unbind(boutonOngletPersonnages, OuvrirOngletPersonnages);

        if (boutonOngletObjets != null)
            UTIL_UiEventBinder.Unbind(boutonOngletObjets, OuvrirOngletObjets);
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
            if (panelTag != null)
            {
                panelRoot = panelTag.gameObject;
            }
            else
            {
                Debug.LogWarning($"PanelRootTag introuvable dans {name}");
            }
        }
    }

    private void BringToFront()
    {
        if (!ordreInitialSauvegarde)
        {
            parentInitial = transform.parent;
            siblingIndexInitial = transform.GetSiblingIndex();
            ordreInitialSauvegarde = true;
        }

        estOuvertEnOverlaySelection = true;

        transform.SetAsLastSibling();

        if (panelRoot != null)
        {
            panelRoot.transform.SetAsLastSibling();
        }

        if (rootCanvas != null)
        {
            rootCanvas.overrideSorting = true;
            rootCanvas.sortingOrder = 100;
        }
    }

    private void RestoreSorting()
    {
        if (ordreInitialSauvegarde && parentInitial == transform.parent && transform.parent != null)
        {
            int maxIndex = transform.parent.childCount - 1;
            int safeIndex = Mathf.Clamp(siblingIndexInitial, 0, maxIndex);
            transform.SetSiblingIndex(safeIndex);
        }

        ordreInitialSauvegarde = false;
        estOuvertEnOverlaySelection = false;

        if (rootCanvas != null && sortingOrderSauvegarde)
        {
            rootCanvas.overrideSorting = false;
            rootCanvas.sortingOrder = sortingOrderInitial;
        }
    }

    public void OpenMenu(
        List<SCOBJ_Personnage> personnages,
        List<SCOBJ_OBJET> objets = null,
        DATA_PERSONNAGE_DisplayContext contexte = null,
        List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = null)
    {
        ConfigurerDonnees(
            personnages,
            objets,
            consommables,
            contexte
        );

        modeAjoutEquipe = false;
        equipeMenuCible = null;
        modeSelectionPersonnage = false;
        onPersonnageSelectionne = null;
        modeSelectionObjetEquipable = false;
        onObjetEquipableSelectionne = null;

        ongletActif = ENUM_INVENTAIRE_Onglet.Personnages;

        OpenPanel();
        RefreshOnglets();
    }

    public void OpenMenuPourEquipe(
        List<SCOBJ_Personnage> personnages,
        UI_EQUIPE_DetailController equipeMenu,
        DATA_PERSONNAGE_DisplayContext contexte = null,
        List<SCOBJ_OBJET> objets = null,
        List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = null)
    {
        ConfigurerDonnees(
            personnages,
            objets,
            consommables,
            contexte
        );

        modeAjoutEquipe = true;
        equipeMenuCible = equipeMenu;
        modeSelectionPersonnage = false;
        onPersonnageSelectionne = null;
        modeSelectionObjetEquipable = false;
        onObjetEquipableSelectionne = null;

        ongletActif = ENUM_INVENTAIRE_Onglet.Personnages;

        OpenPanel();
        RefreshOnglets();
    }

    public void OpenSelectionPersonnage(
        List<SCOBJ_Personnage> personnages,
        Action<SCOBJ_Personnage> onChoisi,
        DATA_PERSONNAGE_DisplayContext contexte = null)
    {
        ConfigurerDonnees(
            personnages,
            new List<SCOBJ_OBJET>(),
            new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),
            contexte
        );

        modeAjoutEquipe = false;
        equipeMenuCible = null;
        modeSelectionPersonnage = true;
        onPersonnageSelectionne = onChoisi;
        modeSelectionObjetEquipable = false;
        onObjetEquipableSelectionne = null;

        ongletActif = ENUM_INVENTAIRE_Onglet.Personnages;

        OpenPanel();
        BringToFront();
        RefreshOnglets();
    }

    public void OpenSelectionObjetEquipable(
        List<SCOBJ_OBJET> objets,
        ENUM_OBJET_EQUIPPABLE typeEquipement,
        Action<SCOBJ_OBJET_EQUIPPABLE> onChoisi,
        DATA_PERSONNAGE_DisplayContext contexte = null)
    {
        ConfigurerDonnees(
            new List<SCOBJ_Personnage>(),
            objets,
            new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),
            contexte
        );

        modeAjoutEquipe = false;
        equipeMenuCible = null;
        modeSelectionPersonnage = false;
        onPersonnageSelectionne = null;
        modeSelectionObjetEquipable = true;
        typeEquipementSelection = typeEquipement;
        onObjetEquipableSelectionne = onChoisi;

        ongletActif = ENUM_INVENTAIRE_Onglet.Objets;

        OpenPanel();
        BringToFront();
        RefreshOnglets();
    }

    private void ConfigurerDonnees(
        List<SCOBJ_Personnage> personnages,
        List<SCOBJ_OBJET> objets,
        List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables,
        DATA_PERSONNAGE_DisplayContext contexte)
    {
        personnagesCourants = personnages ?? new List<SCOBJ_Personnage>();
        objetsCourants = objets ?? new List<SCOBJ_OBJET>();
        consommablesCourants = consommables ?? new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();
        contexteAffichage = contexte ?? DATA_PERSONNAGE_DisplayContext.Default;
    }

    public void CloseMenu()
    {
        ClosePanel();

        modeAjoutEquipe = false;
        equipeMenuCible = null;
        contexteAffichage = DATA_PERSONNAGE_DisplayContext.Default;

        modeSelectionPersonnage = false;
        onPersonnageSelectionne = null;

        modeSelectionObjetEquipable = false;
        onObjetEquipableSelectionne = null;

        consommablesCourants = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();

        if (estOuvertEnOverlaySelection)
        {
            RestoreSorting();
        }
    }

    public void RefreshIfOpen(
        List<SCOBJ_Personnage> personnages,
        List<SCOBJ_OBJET> objets = null,
        List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = null)
    {
        if (!IsOpen())
            return;

        personnagesCourants = personnages ?? new List<SCOBJ_Personnage>();
        objetsCourants = objets ?? new List<SCOBJ_OBJET>();
        consommablesCourants = consommables ?? new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();

        RefreshOnglets();
    }

    public void OuvrirOngletPersonnages()
    {
        ongletActif = ENUM_INVENTAIRE_Onglet.Personnages;
        RefreshOnglets();
    }

    public void OuvrirOngletObjets()
    {
        ongletActif = ENUM_INVENTAIRE_Onglet.Objets;
        RefreshOnglets();
    }

    private void RefreshOnglets()
    {
        RefreshOngletPersonnages();
        RefreshOngletObjets();
    }

    private void RefreshOngletPersonnages()
    {
        if (personnageOngletView == null)
            return;

        bool actif = ongletActif == ENUM_INVENTAIRE_Onglet.Personnages;

        if (actif)
        {
            personnageOngletView.Configure(
                personnagesCourants,
                contexteAffichage,
                equipeMenuCible,
                modeAjoutEquipe,
                modeSelectionPersonnage ? HandlePersonnageSelection : null
            );

            personnageOngletView.Show();
            personnageOngletView.RefreshView();
        }
        else
        {
            personnageOngletView.Hide();
        }
    }

    private void RefreshOngletObjets()
    {
        if (objetOngletView == null)
            return;

        bool actif = ongletActif == ENUM_INVENTAIRE_Onglet.Objets;

        if (actif)
        {
            objetOngletView.SetDetailModeCompact(false);

            objetOngletView.Configure(
                objetsCourants,
                consommablesCourants,
                modeSelectionObjetEquipable ? typeEquipementSelection : (ENUM_OBJET_EQUIPPABLE?)null,
                modeSelectionObjetEquipable ? HandleObjetEquipableSelection : null
            );

            objetOngletView.Show();
            objetOngletView.RefreshView();
        }
        else
        {
            objetOngletView.Hide();
        }
    }

    private void HandlePersonnageSelection(SCOBJ_Personnage personnage)
    {
        if (personnage == null)
            return;

        onPersonnageSelectionne?.Invoke(personnage);
        CloseMenu();
    }

    private void HandleObjetEquipableSelection(SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (objet == null)
            return;

        onObjetEquipableSelectionne?.Invoke(objet);
        CloseMenu();
    }
}