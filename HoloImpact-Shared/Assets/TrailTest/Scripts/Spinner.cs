using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float speed = 100.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}