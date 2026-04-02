using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraZoomController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Camera targetCamera;
[SerializeField] private EquipeMenuController equipeMenuController;
[SerializeField] private PersonnageMenuController personnageMenuController;
    [Header("Zoom")]
    [SerializeField] private float zoomStep = 0.5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float scrollCooldown = 0.05f;

    [Header("Déplacement smooth par direction souris")]
    [SerializeField] private bool enableMouseDirectionMovement = true;
    [SerializeField] private float baseMaxMoveSpeed = 10f;
    [SerializeField] private float baseAcceleration = 20f;
    [SerializeField] private float baseDeceleration = 25f;
    [SerializeField] private float deadZoneRadius = 80f;
    [SerializeField] private float fullSpeedRadius = 300f;

    [Header("Influence du zoom")]
    [SerializeField] private float minSpeedFactor = 0.45f;
    [SerializeField] private float maxDecelerationFactor = 2f;

    [Header("Limites de carte")]
    [SerializeField] private bool useCameraBounds = true;
    [SerializeField] private Vector2 mapMinBounds = new Vector2(-20f, -10f);
    [SerializeField] private Vector2 mapMaxBounds = new Vector2(20f, 10f);

    private float nextAllowedScrollTime = 0f;
    private Vector3 currentVelocity = Vector3.zero;
private bool IsAnyMenuOpen()
{
    bool equipeOpen = equipeMenuController != null && equipeMenuController.IsOpen();
    bool personnageOpen = personnageMenuController != null && personnageMenuController.IsOpen();

    return equipeOpen || personnageOpen;
}
    private void Reset()
    {
        targetCamera = GetComponent<Camera>();
    }

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }
          if (equipeMenuController == null)
    {
        equipeMenuController = FindAnyObjectByType<EquipeMenuController>(FindObjectsInactive.Include);
    }

    if (personnageMenuController == null)
    {
        personnageMenuController = FindAnyObjectByType<PersonnageMenuController>(FindObjectsInactive.Include);
    }
    }

private void Update()
{
    if (targetCamera == null || !targetCamera.orthographic)
    {
        return;
    }

    if (IsAnyMenuOpen())
    {
        currentVelocity = Vector3.zero;
        return;
    }

    HandleZoom();
    HandleSmoothMouseDirectionMovement();
    ClampCameraPosition();
}

    private void HandleZoom()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Time.unscaledTime < nextAllowedScrollTime)
        {
            return;
        }

        float scrollY = Mouse.current.scroll.ReadValue().y;

        if (scrollY > 0f)
        {
            targetCamera.orthographicSize = Mathf.Clamp(
                targetCamera.orthographicSize - zoomStep,
                minZoom,
                maxZoom
            );

            nextAllowedScrollTime = Time.unscaledTime + scrollCooldown;
        }
        else if (scrollY < 0f)
        {
            targetCamera.orthographicSize = Mathf.Clamp(
                targetCamera.orthographicSize + zoomStep,
                minZoom,
                maxZoom
            );

            nextAllowedScrollTime = Time.unscaledTime + scrollCooldown;
        }
    }

    private void HandleSmoothMouseDirectionMovement()
    {
        float zoomT = Mathf.InverseLerp(minZoom, maxZoom, targetCamera.orthographicSize);

        float speedFactor = Mathf.Lerp(1f, minSpeedFactor, zoomT);
        float decelerationFactor = Mathf.Lerp(1f, maxDecelerationFactor, zoomT);

        float currentMaxMoveSpeed = baseMaxMoveSpeed * speedFactor;
        float currentAcceleration = baseAcceleration * speedFactor;
        float currentDeceleration = baseDeceleration * decelerationFactor;

        if (!enableMouseDirectionMovement || Mouse.current == null)
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                currentDeceleration * Time.deltaTime
            );

            transform.position += currentVelocity * Time.deltaTime;
            return;
        }

        // Bloque le déplacement caméra si la souris est au-dessus de l'UI / HUD
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                currentDeceleration * Time.deltaTime
            );

            transform.position += currentVelocity * Time.deltaTime;
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 offset = mousePosition - screenCenter;

        Vector3 targetVelocity = Vector3.zero;
        float distanceFromCenter = offset.magnitude;

        if (distanceFromCenter > deadZoneRadius)
        {
            Vector2 direction = offset.normalized;

            float intensity = Mathf.InverseLerp(deadZoneRadius, fullSpeedRadius, distanceFromCenter);
            intensity = Mathf.Clamp01(intensity);

            targetVelocity = new Vector3(direction.x, direction.y, 0f) * currentMaxMoveSpeed * intensity;
        }

        float speedChange = (targetVelocity == Vector3.zero) ? currentDeceleration : currentAcceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            speedChange * Time.deltaTime
        );

        transform.position += currentVelocity * Time.deltaTime;
    }

    private void ClampCameraPosition()
    {
        if (!useCameraBounds)
        {
            return;
        }

        float cameraHalfHeight = targetCamera.orthographicSize;
        float cameraHalfWidth = targetCamera.orthographicSize * targetCamera.aspect;

        float minX = mapMinBounds.x + cameraHalfWidth;
        float maxX = mapMaxBounds.x - cameraHalfWidth;
        float minY = mapMinBounds.y + cameraHalfHeight;
        float maxY = mapMaxBounds.y - cameraHalfHeight;

        Vector3 clampedPosition = transform.position;

        // Si la caméra est plus large/haute que la carte, on la centre
        if (minX > maxX)
        {
            clampedPosition.x = (mapMinBounds.x + mapMaxBounds.x) * 0.5f;
        }
        else
        {
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        }

        if (minY > maxY)
        {
            clampedPosition.y = (mapMinBounds.y + mapMaxBounds.y) * 0.5f;
        }
        else
        {
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        }

        clampedPosition.z = transform.position.z;
        transform.position = clampedPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (!useCameraBounds)
        {
            return;
        }

        Gizmos.color = Color.green;

        Vector3 center = new Vector3(
            (mapMinBounds.x + mapMaxBounds.x) * 0.5f,
            (mapMinBounds.y + mapMaxBounds.y) * 0.5f,
            0f
        );

        Vector3 size = new Vector3(
            mapMaxBounds.x - mapMinBounds.x,
            mapMaxBounds.y - mapMinBounds.y,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
}