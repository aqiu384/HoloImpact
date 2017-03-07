using UnityEngine;

public class CameraDolly : MonoBehaviour
{

    void Update()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        var cx = Input.GetAxis("CameraHorizontal") * Time.deltaTime * 150.0f;

        transform.Translate(x, 0, 0);
        transform.Translate(0, 0, z);
        transform.Rotate(0, cx, 0);
    }
}