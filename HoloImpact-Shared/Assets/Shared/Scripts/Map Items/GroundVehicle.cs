using UnityEngine;

/// <summary>
/// Extends vehicle and ensures it sticks to surface of terrain.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GroundVehicle : Vehicle
{
    private Vector3 m_groundOffset;

    protected virtual void Awake()
    {
        m_groundOffset = Vector3.up * GetComponent<Collider>().bounds.extents.y;
    }

    protected override void Update()
    {
        base.Update();

        RaycastHit hit;
        if (Physics.Raycast(
            transform.position + Vector3.up, 
            Vector3.down, 
            out hit, 
            Mathf.Infinity, 
            Constants.MAP_TERRAIN_MASK))
        {
            transform.position = hit.point + m_groundOffset;
        }
    }
}