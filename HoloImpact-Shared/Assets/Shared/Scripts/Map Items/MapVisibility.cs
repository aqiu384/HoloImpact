using UnityEngine;

/// <summary>
/// Toggles visibility of map items when they cross the map window boundary.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapVisibility : MonoBehaviour
{
    public delegate void OnParentEnterMap();
    public OnParentEnterMap onParentEnterMap;

    public delegate void OnParentExitMap();
    public OnParentExitMap onParentExitMap;

    protected virtual void Start()
    {
        OnExitMap();
    }

    public virtual void OnEnterMap()
    {
        gameObject.layer = Constants.MAP_VISIBLE_LAYER;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public virtual void OnExitMap()
    {
        gameObject.layer = Constants.MAP_INVISIBLE_LAYER;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}