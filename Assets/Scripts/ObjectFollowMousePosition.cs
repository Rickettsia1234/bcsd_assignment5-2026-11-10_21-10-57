using UnityEngine;

public class ObjectFollowMousePosition : MonoBehaviour
{
    private ObjectDetector objectDetector;

    public void Setup(ObjectDetector objectDetector)
    {
        this.objectDetector = objectDetector;
    }

    private void Update()
    {
        if (objectDetector == null) return;

        transform.position = objectDetector.GetPointerWorldPosition();
    }
}