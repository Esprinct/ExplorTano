using System.Collections.Generic;
using UnityEngine;

public class UI_PROVINCE_EquipeMarkerController : MonoBehaviour
{
    [Header("Marqueurs équipes")]
    [SerializeField] private Transform markerRoot;
    [SerializeField] private UI_EquipeProvinceMarker markerTemplate;
    [SerializeField] private float markerSpacing = 1f;
    [SerializeField] private float markerYOffset = 0.8f;
    [SerializeField] private int maxMarkersVisibles = 8;

    [Header("Debug")]
    [SerializeField] private bool afficherLogs = false;

    private readonly List<UI_EquipeProvinceMarker> markers = new();

    private void Awake()
    {
        if (markerRoot == null)
            markerRoot = transform;

        if (markerTemplate != null)
            markerTemplate.Hide();
    }

    public void RefreshMarkers(STATE_PROVINCE province, SYS_GameManager gameManager)
    {
        if (markerRoot == null)
            markerRoot = transform;

        if (markerTemplate == null)
        {
            if (afficherLogs)
                Debug.LogWarning($"[MARKER] {name} : Marker Template non assigné.");

            return;
        }

        EnsurePool();
        HideAllMarkers();

        if (province == null)
        {
            Log("province null");
            return;
        }

        if (gameManager == null)
        {
            Log("gameManager null");
            return;
        }

        if (gameManager.EquipesRuntime == null)
        {
            Log("EquipesRuntime null");
            return;
        }

        List<STATE_EQUIPE> equipesSurProvince = new();

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null)
                continue;

            if (EstEquipeSurProvince(equipe, province))
                equipesSurProvince.Add(equipe);
        }

        int total = Mathf.Min(equipesSurProvince.Count, maxMarkersVisibles);

        if (afficherLogs)
        {
            Debug.Log(
                $"[MARKER REFRESH] province={province.data?.nom} | " +
                $"equipesSurProvince={equipesSurProvince.Count} | affichées={total} | pool={markers.Count}"
            );
        }

        for (int i = 0; i < total; i++)
        {
            UI_EquipeProvinceMarker marker = markers[i];
            STATE_EQUIPE equipe = equipesSurProvince[i];

            if (marker == null || equipe == null)
                continue;

            marker.transform.localPosition = CalculateMarkerLocalPosition(i, total);
            marker.transform.localRotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one;

            string nomEquipe = equipe.data != null && !string.IsNullOrWhiteSpace(equipe.data.nomEquipe)
                ? equipe.data.nomEquipe
                : "Équipe";

            marker.Setup(equipe.compagnie, nomEquipe);
        }
    }

    private void EnsurePool()
    {
        if (markerRoot == null || markerTemplate == null)
            return;

        markerTemplate.Hide();

        int cible = Mathf.Max(0, maxMarkersVisibles);

        while (markers.Count < cible)
        {
            UI_EquipeProvinceMarker marker = Instantiate(markerTemplate, markerRoot);
            marker.name = $"EquipeMarker_Runtime_{markers.Count}";
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localRotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one;
            marker.Hide();

            markers.Add(marker);
        }
    }

    private void HideAllMarkers()
    {
        if (markerTemplate != null)
            markerTemplate.Hide();

        foreach (UI_EquipeProvinceMarker marker in markers)
        {
            if (marker != null)
                marker.Hide();
        }
    }

    private bool EstEquipeSurProvince(STATE_EQUIPE equipe, STATE_PROVINCE province)
    {
        if (equipe == null || province == null)
            return false;

        if (equipe.provinceAffectee == province)
            return true;

        if (equipe.provinceAffectee == null)
            return false;

        if (equipe.provinceAffectee.data != null &&
            province.data != null &&
            equipe.provinceAffectee.data == province.data)
        {
            return true;
        }

        return false;
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

    private void Log(string message)
    {
        if (!afficherLogs)
            return;

        Debug.LogWarning($"[MARKER] {name} : {message}");
    }
}