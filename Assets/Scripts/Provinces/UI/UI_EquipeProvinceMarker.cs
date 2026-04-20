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

    private Sprite[] framesActuelles;
    private float timer;
    private int frameIndex;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (spriteRenderer == null || framesActuelles == null || framesActuelles.Length == 0)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, fps);

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex++;

            if (frameIndex >= framesActuelles.Length)
                frameIndex = 0;

            spriteRenderer.sprite = framesActuelles[frameIndex];
        }
    }

    public void Setup(ENUM_Compagnie compagnie, string nomEquipe)
    {
        framesActuelles = GetFrames(compagnie);
        frameIndex = 0;
        timer = 0f;

        bool visible = framesActuelles != null && framesActuelles.Length > 0;
        gameObject.SetActive(visible);

        if (visible && spriteRenderer != null)
            spriteRenderer.sprite = framesActuelles[0];

        if (nomEquipeText != null)
            nomEquipeText.text = string.IsNullOrWhiteSpace(nomEquipe) ? "Équipe" : nomEquipe;
    }

    public void Hide()
    {
        framesActuelles = null;
        frameIndex = 0;
        timer = 0f;

        if (nomEquipeText != null)
            nomEquipeText.text = string.Empty;

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
}