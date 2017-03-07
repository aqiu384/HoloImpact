using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Notifies Vehicle Selection Manager when the corresponding vehicle selection menu item is chosen.
/// </summary>
public class VehicleTrackEntry: MonoBehaviour
{
    public VehicleTrackMenu trackMenu;
    public Text nameText;

    private Vehicle m_track;
    public Vehicle track
    {
        get { return m_track; }
        set {
            m_track = value;
            nameText.text = string.Format("Jump to V{0}", m_track.GetID());
        }
    }

    public void OnJumpButtonClicked()
    {
        trackMenu.SelectItem(track);
    }
}