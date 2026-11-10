using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;

    private Camera mainCamera;
    private Vector2 rayPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        rayPosition = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if ( context.ReadValueAsButton() )
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(rayPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, 0f);

            if ( hit.transform != null )
            {
                if ( hit.transform.CompareTag("Tile") )
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
            }
        }
    }
}