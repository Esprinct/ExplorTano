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
    [SerializeField] private UI_PROVINCE_EquipeMarkerController equipeMarkerController;

    [Header("Références")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer claimRenderer;
    [SerializeField] private SpriteRenderer overlayRenderer;
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private SpriteRenderer explorationContourRenderer;
    [SerializeField] private Material hachureMaterial;
    [SerializeField] private Material explorationContourMaterial;
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
    [SerializeField] private float stripeWidthPixels = 20f;
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
    private Material explorationContourMaterialInstance;
private MaterialPropertyBlock explorationContourPropertyBlock;
    public string NomProvince => STATE_PROVINCE != null && STATE_PROVINCE.data != null
        ? STATE_PROVINCE.data.nom
        : nomProvince;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        if (mapController == null)
            mapController = FindAnyObjectByType<MapController>();

        if (equipeMarkerController == null)
            equipeMarkerController = GetComponentInChildren<UI_PROVINCE_EquipeMarkerController>(true);

        if (explorationContourRenderer == null)
            explorationContourRenderer = transform.Find("ExplorationContour")?.GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (polygonCollider == null)
            polygonCollider = GetComponent<PolygonCollider2D>();

        if (equipeMarkerController == null)
            equipeMarkerController = GetComponentInChildren<UI_PROVINCE_EquipeMarkerController>(true);

        if (explorationContourRenderer == null)
            explorationContourRenderer = transform.Find("ExplorationContour")?.GetComponent<SpriteRenderer>();

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

        SYS_GameManager gameManager = GetGameManager();
        gameManager?.RegisterProvince(STATE_PROVINCE);

        RefreshVisual();
    }

    private void Start()
    {
        RefreshEquipeMarkersPublic();
    }

    private void OnDestroy()
    {
        if (overlayMaterialInstance != null)
        {
            Destroy(overlayMaterialInstance);
            overlayMaterialInstance = null;
        }

        if (explorationContourMaterialInstance != null)
        {
            Destroy(explorationContourMaterialInstance);
            explorationContourMaterialInstance = null;
        }
    }

    private void ResolveDependencies()
    {
        if (mapController == null)
            mapController = FindAnyObjectByType<MapController>();

        if (gameManagerCache == null)
            gameManagerCache = FindAnyObjectByType<SYS_GameManager>();

        if (equipeDetailControllerCache == null)
        {
            equipeDetailControllerCache =
                FindAnyObjectByType<UI_EQUIPE_DetailController>(FindObjectsInactive.Include);
        }

        if (provinceMenuControllerCache == null)
        {
            provinceMenuControllerCache =
                FindAnyObjectByType<UI_PROVINCE_MenuController>(FindObjectsInactive.Include);
        }

        if (equipeMarkerController == null)
            equipeMarkerController = GetComponentInChildren<UI_PROVINCE_EquipeMarkerController>(true);

        if (explorationContourRenderer == null)
            explorationContourRenderer = transform.Find("ExplorationContour")?.GetComponent<SpriteRenderer>();
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

if (explorationContourRenderer != null)
{
    explorationContourRenderer.sprite = data.sprite;
    explorationContourRenderer.color = Color.white;
    explorationContourRenderer.enabled = false;
    explorationContourRenderer.gameObject.SetActive(false);
    explorationContourRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
    explorationContourRenderer.sortingOrder = spriteRenderer.sortingOrder + 4;

    Shader outlineShader = Shader.Find("Custom/SpriteExplorationOutline");

    if (explorationContourMaterial != null)
    {
        explorationContourMaterialInstance = new Material(explorationContourMaterial);
    }
    else if (outlineShader != null)
    {
        explorationContourMaterialInstance = new Material(outlineShader);
    }

    if (explorationContourMaterialInstance != null)
    {
        if (outlineShader != null &&
            explorationContourMaterialInstance.shader != outlineShader)
        {
            explorationContourMaterialInstance.shader = outlineShader;
        }

        explorationContourRenderer.material = explorationContourMaterialInstance;
    }
    else
    {
        Debug.LogWarning(
            $"UI_PROVINCE_View '{name}' : impossible de créer le material de contour. " +
            $"Vérifie que le shader Custom/SpriteExplorationOutline existe."
        );
    }

    explorationContourPropertyBlock ??= new MaterialPropertyBlock();
}
    }

    private void InitialiserState()
    {
        STATE_PROVINCE = UI_PROVINCE_StateFactory.CreerDepuisData(data);
    }

    private void RebuildCollider()
    {
        UI_PROVINCE_ColliderBuilder.Rebuild(polygonCollider, spriteRenderer, this);
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
        if (IsPointerOverUI())
            return;

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
        AppliquerContourExploration();

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

        RefreshEquipeMarkersPublic();
    }

    public void RefreshEquipeMarkersPublic()
    {
        if (equipeMarkerController == null)
            equipeMarkerController = GetComponentInChildren<UI_PROVINCE_EquipeMarkerController>(true);

        equipeMarkerController?.RefreshMarkers(STATE_PROVINCE, GetGameManager());
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
            claimRenderer.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        Color couleur = GetCouleurBase();
        couleur.a = alphaClaim;
        claimRenderer.color = couleur;
    }

 private void AppliquerContourExploration()
{
    if (explorationContourRenderer == null)
        return;

    // Si la province est claim, on ne montre plus le contour d'exploration.
    bool provinceClaim =
        STATE_PROVINCE != null &&
        STATE_PROVINCE.estClaim &&
        STATE_PROVINCE.proprietaireActuel.HasValue;

    if (provinceClaim)
    {
        DesactiverContourExploration();
        return;
    }

    bool afficherContour = UI_PROVINCE_ExplorationContourResolver.TryGetStyleContour(
        STATE_PROVINCE,
        couleurMaizin,
        couleurKinia,
        couleurJoho,
        out DATA_PROVINCE_ExplorationContourStyle style
    );

    if (!afficherContour)
    {
        DesactiverContourExploration();
        return;
    }

    explorationContourRenderer.gameObject.SetActive(true);
    explorationContourRenderer.enabled = true;
    explorationContourRenderer.sprite = data != null ? data.sprite : explorationContourRenderer.sprite;
    explorationContourRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
    explorationContourRenderer.sortingOrder = spriteRenderer.sortingOrder + 4;

    Color couleurA = style.couleurA;
    Color couleurB = style.couleurB;

    couleurA.a = 1f;
    couleurB.a = 1f;

    float useHachure = style.mode == ENUM_PROVINCE_ExplorationContourMode.Hachure ? 1f : 0f;

    // Fallback visible si le shader ne fonctionne pas.
    explorationContourRenderer.color = couleurA;

    // Bandes plus grosses.
const float stripeWidthExploration = 0.22f;
const float stripeSpacingExploration = 0.44f;

    if (explorationContourMaterialInstance != null)
    {
        explorationContourRenderer.material = explorationContourMaterialInstance;

        explorationContourMaterialInstance.SetFloat("_UseHachure", useHachure);
        explorationContourMaterialInstance.SetColor("_OutlineColor", couleurA);
        explorationContourMaterialInstance.SetColor("_ColorA", couleurA);
        explorationContourMaterialInstance.SetColor("_ColorB", couleurB);
        explorationContourMaterialInstance.SetFloat("_StripeWidth", stripeWidthExploration);
        explorationContourMaterialInstance.SetFloat("_StripeSpacing", stripeSpacingExploration);
        explorationContourMaterialInstance.SetFloat("_StripeAngle", stripeAngle);
    }

    explorationContourPropertyBlock ??= new MaterialPropertyBlock();

    explorationContourRenderer.GetPropertyBlock(explorationContourPropertyBlock);

    explorationContourPropertyBlock.SetFloat("_UseHachure", useHachure);
    explorationContourPropertyBlock.SetColor("_OutlineColor", couleurA);
    explorationContourPropertyBlock.SetColor("_ColorA", couleurA);
    explorationContourPropertyBlock.SetColor("_ColorB", couleurB);
    explorationContourPropertyBlock.SetFloat("_StripeWidth", stripeWidthExploration);
    explorationContourPropertyBlock.SetFloat("_StripeSpacing", stripeSpacingExploration);
    explorationContourPropertyBlock.SetFloat("_StripeAngle", stripeAngle);

    explorationContourRenderer.SetPropertyBlock(explorationContourPropertyBlock);

#if UNITY_EDITOR
    string shaderName =
        explorationContourRenderer.sharedMaterial != null &&
        explorationContourRenderer.sharedMaterial.shader != null
            ? explorationContourRenderer.sharedMaterial.shader.name
            : "null";

    Debug.Log(
        $"[CONTOUR_EXPLORATION] province={NomProvince} | " +
        $"shader={shaderName} | " +
        $"mode={style.mode} | useHachure={useHachure} | " +
        $"claim={provinceClaim} | " +
        $"stripeWidth={stripeWidthExploration} | stripeSpacing={stripeSpacingExploration} | " +
        $"A={couleurA} | B={couleurB} | " +
        $"Maizin={STATE_PROVINCE?.GetExploration(ENUM_Compagnie.Maizin):0.#}% | " +
        $"Kinia={STATE_PROVINCE?.GetExploration(ENUM_Compagnie.Kinia):0.#}% | " +
        $"Joho={STATE_PROVINCE?.GetExploration(ENUM_Compagnie.Joho):0.#}%"
    );
#endif
}

    private void DesactiverContourExploration()
    {
        if (explorationContourRenderer == null)
            return;

        explorationContourRenderer.enabled = false;
        explorationContourRenderer.gameObject.SetActive(false);
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
        return UI_PROVINCE_ColorResolver.GetCouleurBase(
            STATE_PROVINCE,
            couleurMaizin,
            couleurKinia,
            couleurJoho,
            couleurAutre
        );
    }
}