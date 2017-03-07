using UnityEngine;

public class MainCameraChild : MonoBehaviour
{
    protected virtual void Start()
    {
        Transform newParent = null;

        if (Application.isEditor)
        {
            newParent = Camera.main.transform;
        }

        transform.SetParent(newParent, false);
    }
}