using System.Collections;
using UnityEngine;

public class SYS_AutoPlayController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private SYS_GameManager gameManager;

    [Header("Autoplay")]
    [SerializeField] private bool autoPlayEnabled = false;
    [SerializeField] private bool autoStartIfNoHumanPlayer = true;
    [SerializeField] private float delayBetweenTurns = 0.25f;
    [SerializeField] private int maxTurnsPerSession = 1000;

    private Coroutine autoPlayCoroutine;

    public bool IsRunning => autoPlayCoroutine != null;
    public bool AutoPlayEnabled => autoPlayEnabled;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GetComponent<SYS_GameManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<SYS_GameManager>();
        }
    }

    private void Start()
    {
        if (!autoStartIfNoHumanPlayer)
            return;

        if (gameManager == null)
            return;

        if (gameManager.GetHumanPlayer() == null)
        {
            StartAutoPlay();
        }
    }

    public void SetAutoPlayEnabled(bool enabled)
    {
        autoPlayEnabled = enabled;

        if (!enabled)
        {
            StopAutoPlay();
        }
    }

    public void SetDelayBetweenTurns(float delay)
    {
        delayBetweenTurns = Mathf.Max(0f, delay);
    }

    public void StartAutoPlay()
    {
        if (gameManager == null || gameManager.TurnSystem == null)
            return;

        if (autoPlayCoroutine != null)
            return;

        autoPlayEnabled = true;
        autoPlayCoroutine = StartCoroutine(AutoPlayLoop());
    }

    public void StopAutoPlay()
    {
        autoPlayEnabled = false;

        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }
    }

    public void ToggleAutoPlay()
    {
        if (IsRunning)
        {
            StopAutoPlay();
        }
        else
        {
            StartAutoPlay();
        }
    }

    private IEnumerator AutoPlayLoop()
    {
        int turnsProcessed = 0;

        while (autoPlayEnabled &&
               gameManager != null &&
               gameManager.TurnSystem != null &&
               !gameManager.PartieTerminee &&
               turnsProcessed < maxTurnsPerSession)
        {
            gameManager.TourSuivant();
            turnsProcessed++;

            if (delayBetweenTurns > 0f)
            {
                yield return new WaitForSeconds(delayBetweenTurns);
            }
            else
            {
                yield return null;
            }
        }

        autoPlayCoroutine = null;
    }
}