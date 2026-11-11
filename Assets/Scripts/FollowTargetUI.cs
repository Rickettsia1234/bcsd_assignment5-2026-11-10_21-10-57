using UnityEngine;

public class FollowTargetUI : MonoBehaviour
{
    private Transform targetTransform;
    private RectTransform rectTransform;

    public void Setup(Transform target)
    {
        targetTransform = target;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        rectTransform.position = screenPosition;
    }
}