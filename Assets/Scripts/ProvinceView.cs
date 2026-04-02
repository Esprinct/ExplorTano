using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class ProvinceView : MonoBehaviour
{
    [Header("Données de la province")]
    [SerializeField] private string nomProvince;
    [SerializeField] private ProvinceData data;

    [Header("Références")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private MapController mapController;

    [Header("Couleurs des factions")]
    [SerializeField] private Color couleurMaizin = Color.blue;
    [SerializeField] private Color couleurKinia = Color.purple;
    [SerializeField] private Color couleurJoho = Color.green;
    [SerializeField] private Color couleurAutre = Color.gray;

    [Header("Couleurs UI")]
    [SerializeField] private Color couleurNormale = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color couleurSurvol = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color couleurSelection = Color.red;

    public ProvinceState ProvinceState { get; private set; }

    private bool estSelectionnee = false;

    public string NomProvince => ProvinceState != null && ProvinceState.data != null
        ? ProvinceState.data.nom
        : nomProvince;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mapController == null)
        {
            mapController = FindAnyObjectByType<MapController>();
        }
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null && data != null && data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
        }
    }

    private void Awake()
    {
        Debug.Log($"ProvinceView Awake {name} : {Time.realtimeSinceStartup}");
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogError($"ProvinceView '{name}' : aucun SpriteRenderer trouvé.");
            enabled = false;
            return;
        }

        if (mapController == null)
        {
            mapController = FindAnyObjectByType<MapController>();
        }

        if (data == null)
        {
            Debug.LogError($"ProvinceView '{name}' n'a pas de ProvinceData assigné !");
            enabled = false;
            return;
        }

        if (data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
        }

        ProvinceState = new ProvinceState
        {
            data = data,
            proprietaireActuel = "",
            estClaim = false,
            explorationEnCours = false,
            toursRestants = 0,
            influenceMaizin = data.influenceMaizinInitiale,
            influenceJoho = data.influenceJohoInitiale,
            influenceKinia = data.influenceKiniaInitiale,
            influenceAutre = data.influenceAutreInitiale
        };

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.RegisterProvince(ProvinceState);
        }
        else
        {
            Debug.LogWarning("GameManager introuvable lors du RegisterProvince.");
        }

        RefreshVisual();
    }

    private void OnMouseDown()
    {
        Debug.Log("Province cliquée : " + NomProvince);

        if (mapController != null)
        {
            mapController.SelectionnerProvince(this);
        }
        else
        {
            Selectionner();
        }

        ProvinceMenuController provinceMenuController = FindAnyObjectByType<ProvinceMenuController>(FindObjectsInactive.Include);
        if (provinceMenuController != null)
        {
            provinceMenuController.OpenProvinceMenu(ProvinceState);
        }
    }

    private void OnMouseEnter()
    {
        if (!estSelectionnee)
        {
            AppliquerCouleurSurvol();
        }
    }

    private void OnMouseExit()
    {
        if (!estSelectionnee)
        {
            RefreshVisual();
        }
    }

    public void Selectionner()
    {
        estSelectionnee = true;
        AppliquerCouleurSelection();
    }

    public void Deselectionner()
    {
        estSelectionnee = false;
        RefreshVisual();
    }

    public bool EstSelectionnee()
    {
        return estSelectionnee;
    }

    public void RefreshVisual()
    {
        if (ProvinceState == null)
        {
            AppliquerCouleurNormale();
            return;
        }

        if (estSelectionnee)
        {
            AppliquerCouleurSelection();
            return;
        }

        AppliquerCouleurNormale();
    }

    private Color GetCouleurBase()
    {
        if (ProvinceState == null || !ProvinceState.estClaim)
        {
            return couleurNormale;
        }

        switch (ProvinceState.proprietaireActuel)
        {
            case "Maizin":
                return couleurMaizin;

            case "Kinia":
                return couleurKinia;

            case "Joho":
                return couleurJoho;

            default:
                return couleurAutre;
        }
    }

    private Color GetCouleurSurvol()
    {
        Color baseColor = GetCouleurBase();

        if (ProvinceState == null || !ProvinceState.estClaim)
        {
            return Color.Lerp(baseColor, Color.black, 0.15f);
        }

        return Color.Lerp(baseColor, Color.white, 0.25f);
    }

    private Color GetCouleurSelection()
    {
        Color baseColor = GetCouleurBase();

        if (ProvinceState == null || !ProvinceState.estClaim)
        {
            return Color.Lerp(baseColor, Color.red, 0.5f);
        }

        return Color.Lerp(baseColor, Color.black, 0.25f);
    }

    private void AppliquerCouleurNormale()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetCouleurBase();
        }
    }

    private void AppliquerCouleurSurvol()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetCouleurSurvol();
        }
    }

    private void AppliquerCouleurSelection()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetCouleurSelection();
        }
    }
}