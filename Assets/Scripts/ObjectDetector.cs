using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner    towerSpawner;
    [SerializeField]
    private UIPopupTower    uiPopupTower;

    private Camera          mainCamera;
    private Ray             ray;
    private RaycastHit      hit;
    private Vector2         pointerPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        pointerPosition = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if ( context.performed && context.ReadValueAsButton() )
        {
            ray = mainCamera.ScreenPointToRay(pointerPosition);

            if ( Physics.Raycast(ray, out hit, Mathf.Infinity) )
            {
                if ( hit.transform.CompareTag("Tile") )
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
                else if ( hit.transform.CompareTag("Tower") )
                {
                    uiPopupTower.OnPopup(hit.transform);
                }
            }
        }
    }
}