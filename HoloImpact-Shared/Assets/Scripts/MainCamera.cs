using UnityEngine;

public class MainCamera : MonoBehaviour
{
    void Update()
    {
        var cz = Input.GetAxis("CameraVertical") * Time.deltaTime * -150.0f;

        transform.Rotate(cz, 0, 0);
    }
}
