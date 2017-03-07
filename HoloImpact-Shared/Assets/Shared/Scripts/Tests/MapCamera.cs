using UnityEngine;

public class MapCamera : MonoBehaviour
{
    public Transform mainMap;
    public float offsetAngle = 0;

    protected virtual void Update()
    {
        var mainCamera = Camera.main.transform;
        transform.position = mainCamera.position;
        transform.rotation = mainCamera.rotation;

        transform.RotateAround(mainMap.position, mainMap.up, offsetAngle);
    }
}