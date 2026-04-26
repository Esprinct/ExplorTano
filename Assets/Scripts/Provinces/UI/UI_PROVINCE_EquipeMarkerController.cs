using System.Collections.Generic;
using UnityEngine;

public class UI_PROVINCE_EquipeMarkerController : MonoBehaviour
{
    [Header("Marqueurs équipes")]
    [SerializeField] private Transform markerRoot;
    [SerializeField] private UI_EquipeProvinceMarker markerPrefab;
    [SerializeField] private float markerSpacing = 0.9f;
    [SerializeField] private float markerYOffset = 0.8f;
    [SerializeField] private int maxMarkersVisibles = 8;

    [Header("Debug")]
    [SerializeField] private bool afficherLogs = false;

    private readonly List<UI_EquipeProvinceMarker> markersEquipe = new();

    private STATE_PROVINCE province;
    private SYS_GameManager gameManager;

    public void Setup(STATE_PROVINCE provinceSource, SYS_GameManager gm)
    {
        province = provinceSource;
        gameManager = gm;

        EnsureMarkerPool();
        HideAll();
        Refresh();
    }

    public void Refresh()
    {
        if (markerRoot == null || markerPrefab == null)
            return;

        EnsureMarkerPool();
        HideAll();

        if (province == null || gameManager == null || gameManager.EquipesRuntime == null)
            return;

        List<STATE_EQUIPE> equipesSurProvince = new();

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null)
                continue;

            if (equipe.provinceAffectee != province)
                continue;

            equipesSurProvince.Add(equipe);
        }

        if (afficherLogs)
        {
            Debug.Log(
                $"[MARKER RESULT] province={province.data?.nom} | " +
                $"équipes trouvées={equipesSurProvince.Count} | pool={markersEquipe.Count}"
            );
        }

        int total = Mathf.Min(equipesSurProvince.Count, maxMarkersVisibles);

        for (int i = 0; i < total; i++)
        {
            STATE_EQUIPE equipe = equipesSurProvince[i];
            UI_EquipeProvinceMarker marker = markersEquipe[i];

            if (equipe == null || marker == null)
                continue;

            marker.transform.localPosition = CalculateMarkerLocalPosition(i, total);
            marker.transform.localRotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one;

            string nomEquipe =
                equipe.data != null && !string.IsNullOrWhiteSpace(equipe.data.nomEquipe)
                    ? equipe.data.nomEquipe
                    : "Équipe";

            marker.Setup(equipe.compagnie, nomEquipe);
        }
    }

    public void HideAll()
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