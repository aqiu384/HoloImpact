using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains an authoritative list of all map items that should be included by listeners.
/// </summary>
public abstract class SelectorController<T> : MonoBehaviour where T : IMapItem
{
    private HashSet<T> m_mapItems = new HashSet<T>();

    public delegate void MapItemDelegate(T mapItem);
    public MapItemDelegate onItemAddedDelegate, onItemSelectedDelgate, onItemDeletedDelegate;

    public IEnumerable<T> GetMapItems()
    {
        return m_mapItems;
    }

    public bool AddItem(T mapItem)
    {
        if (m_mapItems.Add(mapItem))
        {
            if (onItemAddedDelegate != null) onItemAddedDelegate(mapItem);
            return true;
        }

        return false;
    }

    public virtual void SelectItem(T mapItem)
    {
        if (onItemSelectedDelgate != null) onItemSelectedDelgate(mapItem);
    }

    public bool DeleteItem(T mapItem)
    {
        if (m_mapItems.Remove(mapItem))
        {
            if (onItemDeletedDelegate != null) onItemDeletedDelegate(mapItem);
            return true;
        }

        return false;
    }
}