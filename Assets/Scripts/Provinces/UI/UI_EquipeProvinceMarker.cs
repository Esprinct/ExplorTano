using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UI_EquipeProvinceMarker : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text nomEquipeText;

    [Header("Sprites animés")]
    [SerializeField] private Sprite[] framesMaizin;
    [SerializeField] private Sprite[] framesKinia;
    [SerializeField] private Sprite[] framesJoho;

    [Header("Animation")]
    [SerializeField] private float fps = 6f;

    [Header("Affichage")]
    [SerializeField] private int sortingOrder = 50;
    [SerializeField] private bool cacherAuDemarrage = true;

    private Sprite[] framesActuelles;
    private Sprite spriteFallback;
    private float timer;
    private int frameIndex;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteFallback = spriteRenderer.sprite;

        AppliquerSorting();

        if (cacherAuDemarrage)
            Hide();
    }

    private void Update()
    {
        if (spriteRenderer == null || framesActuelles == null || framesActuelles.Length == 0)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, fps);

        if (timer < frameDuration)
            return;

        timer -= frameDuration;
        frameIndex++;

        if (frameIndex >= framesActuelles.Length)
            frameIndex = 0;

        spriteRenderer.sprite = framesActuelles[frameIndex];
    }

    public void Setup(ENUM_Compagnie compagnie, string nomEquipe)
    {
        framesActuelles = GetFrames(compagnie);
        frameIndex = 0;
        timer = 0f;

        gameObject.SetActive(true);

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            AppliquerSorting();

            if (framesActuelles != null && framesActuelles.Length > 0)
                spriteRenderer.sprite = framesActuelles[0];
            else
                spriteRenderer.sprite = spriteFallback;
        }

        if (nomEquipeText != null)
        {
            nomEquipeText.gameObject.SetActive(true);
            nomEquipeText.text = string.IsNullOrWhiteSpace(nomEquipe) ? "Équipe" : nomEquipe;
        }

        Debug.Log(
            $"[MARKER SETUP] marker={name} | compagnie={compagnie} | " +
            $"frames={(framesActuelles != null ? framesActuelles.Length : 0)} | " +
            $"spriteVisible={(spriteRenderer != null && spriteRenderer.sprite != null)}"
        );
    }

    public void Hide()
    {
        framesActuelles = null;
        frameIndex = 0;
        timer = 0f;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (nomEquipeText != null)
        {
            nomEquipeText.text = "";
            nomEquipeText.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    private Sprite[] GetFrames(ENUM_Compagnie compagnie)
    {
        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                return framesMaizin;

            case ENUM_Compagnie.Kinia:
                return framesKinia;

            case ENUM_Compagnie.Joho:
                return framesJoho;

            default:
                return null;
        }
    }

    private void AppliquerSorting()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = sortingOrder;
        }

        if (nomEquipeText != null)
        {
            Renderer textRenderer = nomEquipeText.GetComponent<Renderer>();
            if (textRenderer != null)
            {
                textRenderer.sortingLayerName = "Default";
                textRenderer.sortingOrder = sortingOrder + 1;
            }
        }
    }
}