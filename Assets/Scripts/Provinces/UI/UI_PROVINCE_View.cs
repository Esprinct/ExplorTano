using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class UI_PROVINCE_View : MonoBehaviour
{
    [Header("Données de la province")]
    [SerializeField] private string nomProvince;
    [SerializeField] private SCOBJ_PROVINCE data;

    [Header("Marqueurs équipes")]
    [SerializeField] private Transform markerRoot;
    [SerializeField] private UI_EquipeProvinceMarker markerPrefab;
    [SerializeField] private float markerSpacing = 0.9f;
    [SerializeField] private float markerYOffset = 0.8f;
    [SerializeField] private int maxMarkersVisibles = 8;

    [Header("Références")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer claimRenderer;
    [SerializeField] private SpriteRenderer overlayRenderer;
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private Material hachureMaterial;
    [SerializeField] private MapController mapController;

    [Header("Couleurs des factions")]
    [SerializeField] private Color couleurMaizin = Color.blue;
    [SerializeField] private Color couleurKinia = new Color(0.6f, 0f, 1f, 1f);
    [SerializeField] private Color couleurJoho = Color.green;
    [SerializeField] private Color couleurAutre = Color.gray;

    [Header("Couleurs UI")]
    [SerializeField] private Color couleurSurvol = Color.white;
    [SerializeField] private Color couleurSelection = Color.red;
    [SerializeField, Range(0f, 1f)] private float alphaSurvol = 0.20f;
    [SerializeField, Range(0f, 1f)] private float alphaSelection = 0.30f;

    [Header("Couleur de claim")]
    [SerializeField, Range(0f, 1f)] private float alphaClaim = 1f;

    [Header("Réglages hachures 50/50")]
    [SerializeField] private float stripeWidthPixels = 4f;
    [SerializeField] private float stripeSpacingPixels = 12f;
    [SerializeField] private float stripeAngle = 45f;
    [SerializeField, Range(0f, 1f)] private float alphaMultiplier = 0.9f;

    [Header("Tolérance ratio 50/50")]
    [SerializeField] private float toleranceRatio = 0.001f;

    public STATE_PROVINCE STATE_PROVINCE { get; private set; }

    private bool estSelectionnee;
    private bool estSurvolee;
    private PolygonCollider2D polygonCollider;

    private SYS_GameManager gameManagerCache;
    private UI_EQUIPE_DetailController equipeDetailControllerCache;
    private UI_PROVINCE_MenuController provinceMenuControllerCache;

    private Material overlayMaterialInstance;
    private readonly List<UI_EquipeProvinceMarker> markersEquipe = new();

    public string NomProvince => STATE_PROVINCE != null && STATE_PROVINCE.data != null
        ? STATE_PROVINCE.data.nom
        : nomProvince;
private void Start()
{
    RefreshEquipeMarkers();
}
    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        if (mapController == null)
            mapController = FindAnyObjectByType<MapController>();
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (polygonCollider == null)
            polygonCollider = GetComponent<PolygonCollider2D>();

        if (spriteRenderer != null && data != null && data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
            RebuildCollider();
        }
    }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (polygonCollider == null)
            polygonCollider = GetComponent<PolygonCollider2D>();

        if (spriteRenderer == null || polygonCollider == null)
        {
            Debug.LogError($"UI_PROVINCE_View '{name}' : références principales manquantes.");
            enabled = false;
            return;
        }

        ResolveDependencies();

        if (data == null)
        {
            Debug.LogError($"UI_PROVINCE_View '{name}' n'a pas de SCOBJ_PROVINCE assigné.");
            enabled = false;
            return;
        }

        if (data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
            RebuildCollider();
        }

        InitialiserRenderers();
        InitialiserState();

        GetGameManager()?.RegisterProvince(STATE_PROVINCE);

        EnsureMarkerPool();
        CacherTousLesMarkersEquipe();

        RefreshVisual();
    }

    private void OnDestroy()
    {
        if (overlayMaterialInstance != null)
        {
            Destroy(overlayMaterialInstance);
            overlayMaterialInstance = null;
        }
    }

    private void ResolveDependencies()
    {
        if (mapController == null)
            mapController = FindAnyObjectByType<MapController>();

        if (gameManagerCache == null)
            gameManagerCache = FindAnyObjectByType<SYS_GameManager>();

        if (equipeDetailControllerCache == null)
            equipeDetailControllerCache =
                FindAnyObjectByType<UI_EQUIPE_DetailController>(FindObjectsInactive.Include);

        if (provinceMenuControllerCache == null)
            provinceMenuControllerCache =
                FindAnyObjectByType<UI_PROVINCE_MenuController>(FindObjectsInactive.Include);
    }

    private SYS_GameManager GetGameManager()
    {
        if (gameManagerCache == null)
            gameManagerCache = FindAnyObjectByType<SYS_GameManager>();

        return gameManagerCache;
    }

    private void InitialiserRenderers()
    {
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Default");
        spriteRenderer.sortingOrder = 0;

        if (claimRenderer != null)
        {
            claimRenderer.sprite = data.sprite;
            claimRenderer.color = new Color(1f, 1f, 1f, 0f);
            claimRenderer.enabled = true;
            claimRenderer.gameObject.SetActive(true);
            claimRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            claimRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        }

        if (overlayRenderer != null)
        {
            overlayRenderer.sprite = data.sprite;
            overlayRenderer.color = Color.white;
            overlayRenderer.enabled = false;
            overlayRenderer.gameObject.SetActive(false);
            overlayRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            overlayRenderer.sortingOrder = spriteRenderer.sortingOrder + 2;

            if (hachureMaterial != null)
            {
                overlayMaterialInstance = new Material(hachureMaterial);
                overlayRenderer.material = overlayMaterialInstance;
            }
            else
            {
                Debug.LogWarning($"UI_PROVINCE_View '{name}' : hachureMaterial non assigné.");
            }
        }

        if (highlightRenderer != null)
        {
            highlightRenderer.sprite = data.sprite;
            highlightRenderer.color = new Color(1f, 1f, 1f, 0f);
            highlightRenderer.enabled = false;
            highlightRenderer.gameObject.SetActive(false);
            highlightRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            highlightRenderer.sortingOrder = spriteRenderer.sortingOrder + 3;
        }
    }

    private void InitialiserState()
    {
        STATE_PROVINCE = new STATE_PROVINCE
        {
            data = data,
            proprietaireActuel = null,
            estClaim = false,
            explorationEnCours = false,
            toursRestants = 0,
            influenceMaizin = data.influenceMaizinInitiale,
            influenceKinia = data.influenceKiniaInitiale,
            influenceJoho = data.influenceJohoInitiale,
            influenceAutre = data.influenceAutreInitiale
        };
    }

    private void RebuildCollider()
    {
        if (polygonCollider == null || spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        polygonCollider.pathCount = 0;

        int shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
        if (shapeCount <= 0)
        {
            Debug.LogWarning($"UI_PROVINCE_View '{name}' : le sprite n'a pas de Physics Shape.");
            return;
        }

        polygonCollider.pathCount = shapeCount;

        List<Vector2> shape = new();

        for (int i = 0; i < shapeCount; i++)
        {
            shape.Clear();
            spriteRenderer.sprite.GetPhysicsShape(i, shape);
            polygonCollider.SetPath(i, shape);
        }
    }

private void OnMouseDown()
{
    if (IsPointerOverUI())
        return;

    ResolveDependencies();

    if (mapController != null)
        mapController.SelectionnerProvince(this);
    else
        Selectionner();

    if (equipeDetailControllerCache != null &&
        equipeDetailControllerCache.EstEnAttenteSelectionProvince)
    {
        equipeDetailControllerCache.OnProvinceCliqueePourAffectation(STATE_PROVINCE);
        return;
    }

    if (provinceMenuControllerCache != null)
        provinceMenuControllerCache.OpenProvinceMenu(STATE_PROVINCE);
}

private bool IsPointerOverUI()
{
    if (EventSystem.current == null)
        return false;

    return EventSystem.current.IsPointerOverGameObject();
}

    private void OnMouseEnter()
    {
        estSurvolee = true;
        RefreshVisual();
    }

    private void OnMouseExit()
    {
        estSurvolee = false;
        RefreshVisual();
    }

    public void Selectionner()
    {
        estSelectionnee = true;
        estSurvolee = false;
        RefreshVisual();
    }

    public void Deselectionner()
    {
        estSelectionnee = false;
        estSurvolee = false;
        RefreshVisual();
    }

    public bool EstSelectionnee()
    {
        return estSelectionnee;
    }

    public void RefreshVisual()
    {
        AppliquerClaimOverlay();

        bool provinceContestee = AfficherProvinceContestee5050();

        if (estSelectionnee)
        {
            AppliquerSelection();
        }
        else if (estSurvolee)
        {
            AppliquerSurvol();
        }
        else
        {
            CacherHighlight();

            if (!provinceContestee)
                DesactiverOverlay();
        }

        RefreshEquipeMarkers();
    }

    private void AppliquerClaimOverlay()
    {
        if (claimRenderer == null)
            return;

        claimRenderer.sprite = data != null ? data.sprite : claimRenderer.sprite;
        claimRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        claimRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        claimRenderer.enabled = true;

        bool afficherClaim =
            STATE_PROVINCE != null &&
            STATE_PROVINCE.estClaim &&
            STATE_PROVINCE.proprietaireActuel.HasValue;

        if (!afficherClaim)
        {
            Color invisible = claimRenderer.color;
            invisible.r = 1f;
            invisible.g = 1f;
            invisible.b = 1f;
            invisible.a = 0f;
            claimRenderer.color = invisible;
            return;
        }

        Color couleur = GetCouleurBase();
        couleur.a = alphaClaim;
        claimRenderer.color = couleur;
    }

    private bool AfficherProvinceContestee5050()
    {
        if (STATE_PROVINCE == null)
        {
            DesactiverOverlay();
            return false;
        }

        float totalInfluence =
            STATE_PROVINCE.influenceMaizin +
            STATE_PROVINCE.influenceKinia +
            STATE_PROVINCE.influenceJoho +
            STATE_PROVINCE.influenceAutre;

        if (totalInfluence <= 0f)
        {
            DesactiverOverlay();
            return false;
        }

        float ratioMaizin = STATE_PROVINCE.influenceMaizin / totalInfluence;
        float ratioKinia = STATE_PROVINCE.influenceKinia / totalInfluence;
        float ratioJoho = STATE_PROVINCE.influenceJoho / totalInfluence;

        bool maizinKinia =
            Mathf.Abs(ratioMaizin - 0.5f) <= toleranceRatio &&
            Mathf.Abs(ratioKinia - 0.5f) <= toleranceRatio;

        bool maizinJoho =
            Mathf.Abs(ratioMaizin - 0.5f) <= toleranceRatio &&
            Mathf.Abs(ratioJoho - 0.5f) <= toleranceRatio;

        bool kiniaJoho =
            Mathf.Abs(ratioKinia - 0.5f) <= toleranceRatio &&
            Mathf.Abs(ratioJoho - 0.5f) <= toleranceRatio;

        bool afficher = maizinKinia || maizinJoho || kiniaJoho;

        if (!afficher)
        {
            DesactiverOverlay();
            return false;
        }

        if (overlayRenderer == null || overlayMaterialInstance == null)
            return false;

        overlayRenderer.gameObject.SetActive(true);
        overlayRenderer.enabled = true;
        overlayRenderer.sprite = data != null ? data.sprite : overlayRenderer.sprite;
        overlayRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        overlayRenderer.sortingOrder = spriteRenderer.sortingOrder + 2;

        Color couleurA = couleurAutre;
        Color couleurB = couleurAutre;

        if (maizinKinia)
        {
            couleurA = couleurMaizin;
            couleurB = couleurKinia;
        }
        else if (maizinJoho)
        {
            couleurA = couleurMaizin;
            couleurB = couleurJoho;
        }
        else if (kiniaJoho)
        {
            couleurA = couleurKinia;
            couleurB = couleurJoho;
        }

        couleurA.a *= alphaMultiplier;
        couleurB.a *= alphaMultiplier;

        overlayMaterialInstance.SetColor("_ColorA", couleurA);
        overlayMaterialInstance.SetColor("_ColorB", couleurB);
        overlayMaterialInstance.SetFloat("_StripeWidth", stripeWidthPixels);
        overlayMaterialInstance.SetFloat("_StripeSpacing", stripeSpacingPixels);
        overlayMaterialInstance.SetFloat("_StripeAngle", stripeAngle);

        return true;
    }

    private void AppliquerSurvol()
    {
        AppliquerHighlight(couleurSurvol, alphaSurvol);
    }

    private void AppliquerSelection()
    {
        AppliquerHighlight(couleurSelection, alphaSelection);
    }

    private void AppliquerHighlight(Color couleur, float alpha)
    {
        if (highlightRenderer == null)
            return;

        highlightRenderer.gameObject.SetActive(true);
        highlightRenderer.enabled = true;
        highlightRenderer.sprite = data != null ? data.sprite : highlightRenderer.sprite;
        highlightRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        highlightRenderer.sortingOrder = spriteRenderer.sortingOrder + 3;

        couleur.a = alpha;
        highlightRenderer.color = couleur;
    }

    private void CacherHighlight()
    {
        if (highlightRenderer == null)
            return;

        highlightRenderer.enabled = false;
        highlightRenderer.gameObject.SetActive(false);
    }

    private void DesactiverOverlay()
    {
        if (overlayRenderer == null)
            return;

        overlayRenderer.enabled = false;
        overlayRenderer.gameObject.SetActive(false);
    }

    private Color GetCouleurBase()
    {
        if (STATE_PROVINCE == null || !STATE_PROVINCE.proprietaireActuel.HasValue)
            return couleurAutre;

        switch (STATE_PROVINCE.proprietaireActuel.Value)
        {
            case ENUM_Compagnie.Maizin:
                return couleurMaizin;

            case ENUM_Compagnie.Kinia:
                return couleurKinia;

            case ENUM_Compagnie.Joho:
                return couleurJoho;

            default:
                return couleurAutre;
        }
    }

    private void EnsureMarkerPool()
    {
        if (markerRoot == null || markerPrefab == null)
            return;

        markerPrefab.Hide();

        while (markersEquipe.Count < maxMarkersVisibles)
        {
            UI_EquipeProvinceMarker marker = Instantiate(markerPrefab, markerRoot);
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localRotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one;

            marker.Hide();
            markersEquipe.Add(marker);
        }
    }

    private void CacherTousLesMarkersEquipe()
    {
        foreach (UI_EquipeProvinceMarker marker in markersEquipe)
        {
            if (marker != null)
                marker.Hide();
        }

        if (markerPrefab != null)
            markerPrefab.Hide();

        if (markerRoot == null)
            return;

        UI_EquipeProvinceMarker[] markersExistants =
            markerRoot.GetComponentsInChildren<UI_EquipeProvinceMarker>(true);

        foreach (UI_EquipeProvinceMarker marker in markersExistants)
        {
            if (marker == null)
                continue;

            if (marker == markerPrefab)
                continue;

            if (!markersEquipe.Contains(marker))
                marker.Hide();
        }
    }

  private void RefreshEquipeMarkers()
{
    if (markerRoot == null)
    {
        Debug.LogWarning($"[MARKER] {name} : markerRoot est null");
        return;
    }

    if (markerPrefab == null)
    {
        Debug.LogWarning($"[MARKER] {name} : markerPrefab est null");
        return;
    }

    EnsureMarkerPool();
    CacherTousLesMarkersEquipe();

    if (STATE_PROVINCE == null)
    {
        Debug.LogWarning($"[MARKER] {name} : STATE_PROVINCE est null");
        return;
    }

    SYS_GameManager gameManager = GetGameManager();

    if (gameManager == null)
    {
        Debug.LogWarning($"[MARKER] {name} : SYS_GameManager introuvable");
        return;
    }

    if (gameManager.EquipesRuntime == null)
    {
        Debug.LogWarning($"[MARKER] {name} : EquipesRuntime est null");
        return;
    }

    List<STATE_EQUIPE> equipesSurProvince = new();

    foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
    {
        if (equipe == null)
            continue;

        string provinceEquipe = equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
            ? equipe.provinceAffectee.data.nom
            : "Aucune";

        string provinceActuelleNom = STATE_PROVINCE.data != null
            ? STATE_PROVINCE.data.nom
            : name;

        Debug.Log(
            $"[MARKER CHECK] provinceActuelle={provinceActuelleNom} | " +
            $"equipe={equipe.data?.nomEquipe} | " +
            $"provinceEquipe={provinceEquipe} | " +
            $"memeReference={(equipe.provinceAffectee == STATE_PROVINCE)}"
        );

        if (equipe.provinceAffectee != STATE_PROVINCE)
            continue;

        equipesSurProvince.Add(equipe);
    }

    Debug.Log(
        $"[MARKER RESULT] province={NomProvince} | équipes trouvées={equipesSurProvince.Count} | " +
        $"markersPool={markersEquipe.Count}"
    );

    int total = Mathf.Min(equipesSurProvince.Count, maxMarkersVisibles);

    for (int i = 0; i < total; i++)
    {
        STATE_EQUIPE equipe = equipesSurProvince[i];

        if (equipe == null || i >= markersEquipe.Count)
            continue;

        UI_EquipeProvinceMarker marker = markersEquipe[i];

        if (marker == null)
            continue;

        marker.transform.localPosition = CalculateMarkerLocalPosition(i, total);
        marker.transform.localRotation = Quaternion.identity;
        marker.transform.localScale = Vector3.one;

        string nomEquipe =
            equipe.data != null && !string.IsNullOrWhiteSpace(equipe.data.nomEquipe)
                ? equipe.data.nomEquipe
                : "Équipe";

        Debug.Log(
            $"[MARKER SHOW] province={NomProvince} | equipe={nomEquipe} | compagnie={equipe.compagnie}"
        );

        marker.Setup(equipe.compagnie, nomEquipe);
    }
}

    private Vector3 CalculateMarkerLocalPosition(int index, int total)
    {
        if (total <= 1)
            return new Vector3(0f, markerYOffset, 0f);

        float largeurTotale = (total - 1) * markerSpacing;
        float startX = -largeurTotale * 0.5f;
        float x = startX + index * markerSpacing;

        return new Vector3(x, markerYOffset, 0f);
    }
}