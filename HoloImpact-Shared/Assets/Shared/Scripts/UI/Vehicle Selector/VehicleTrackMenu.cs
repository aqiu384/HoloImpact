using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Alternative input menu for vehicle selections instead of gaze.
/// Generates selectable entries for all vehicles on the map
/// which send notifications to the vehicle selection manager.
/// </summary>
public class VehicleTrackMenu : MonoBehaviour
{
    public GameObject vehicleTrackEntryPrefab;
    public GameObject targetArea;
    public VehicleSelectorController vehicleSelector;

    private Dictionary<long, GameObject> m_trackEntries;

    protected virtual void Awake()
    {
        m_trackEntries = new Dictionary<long, GameObject>();
    }

    private bool CheckPreconditions()
    {
        if (vehicleTrackEntryPrefab == null ||
            vehicleTrackEntryPrefab.GetComponent<VehicleTrackEntry>() == null ||
            vehicleSelector == null ||
            targetArea == null)
        {
            return false;
        }

        vehicleSelector.onItemAddedDelegate += OnVehicleTrackStart;
        vehicleSelector.onItemDeletedDelegate += OnVehicleTrackDestroy;

        return true;
    }

    protected virtual void Start()
    {
        if (!CheckPreconditions())
        {
            Destroy(gameObject);
        }
        else
        {
            foreach (Vehicle track in vehicleSelector.GetMapItems())
            {
                OnVehicleTrackStart(track);
            }
        }
	}

    protected virtual void OnDestroy()
    {
        vehicleSelector.onItemAddedDelegate -= OnVehicleTrackStart;
        vehicleSelector.onItemDeletedDelegate -= OnVehicleTrackDestroy;
    }
	
	public void OnVehicleTrackStart(Vehicle track)
    {
        var entry = Instantiate(vehicleTrackEntryPrefab);

        var entryInfo = entry.GetComponent<VehicleTrackEntry>();
        entryInfo.track = track;
        entryInfo.trackMenu = this;

        var entryTransform = entry.transform;
        entryTransform.SetParent(targetArea.transform, false);
        entryTransform.localScale = Vector3.one;
        entryTransform.localPosition = Vector3.zero;

        m_trackEntries[track.GetID()] = entry;
    }

    public void OnVehicleTrackDestroy(Vehicle track)
    {
        var entry = m_trackEntries[track.GetID()];
        m_trackEntries.Remove(track.GetID());

        Destroy(entry);
    }

    public void SelectItem(Vehicle vehicle)
    {
        vehicleSelector.SelectItem(vehicle);
    }
}