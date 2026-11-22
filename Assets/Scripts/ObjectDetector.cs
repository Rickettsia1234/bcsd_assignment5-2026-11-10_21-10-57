using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;
    [SerializeField]
    private UIPopupTower uiPopupTower;
    private Camera mainCamera;
    private Vector2 screenPoint;
    private bool isPointerOverUI;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetPointerWorldPosition()
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        worldPoint.z = 0f;

        return worldPoint;
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        screenPoint = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isPointerOverUI) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                towerSpawner.TrySpawnTower(hit.transform);
            }
            else if (hit.transform.CompareTag("Tower"))
            {
                uiPopupTower.OnPopup(hit.transform);
            }
        }
    }
}