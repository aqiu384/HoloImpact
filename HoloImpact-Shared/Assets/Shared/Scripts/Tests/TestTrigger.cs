using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class TestTrigger : MonoBehaviour
{
    private Collider m_collider;

    protected virtual void Awake()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Objected entered");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Object exited");
    }
}