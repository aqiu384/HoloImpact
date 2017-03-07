using UnityEngine;

/// <summary>
/// Spawns four corner tiles around the given central coordinate
/// and downloads the necessary terrain data from the given tile server.
/// </summary>
public class MapTileController : MonoBehaviour
{
    private NetworkManagerFSM m_networkManagerFSM;
    private MapTile[] m_mapTiles;
    private string m_serverPath;
    private Vector2 m_mapCenter = new Vector2(32.7157f, -117.1611f); // San Diego

    public string serverAddress = "172.16.80.1";
    public int serverPort = 7775;
    public MapTile NECorner, SECorner, NWCorner, SWCorner;

    private bool CheckPreconditions()
    {
        var manager = MyNetworkManager.Instance;

        if (manager == null)
        {
            return false;
        }

        m_networkManagerFSM = manager.GetComponent<NetworkManagerFSM>();
        m_mapTiles = new MapTile[]{ NECorner, SECorner, NWCorner, SWCorner };
        m_serverPath = string.Format("http://{0}:{1}/", serverAddress, serverPort);

        foreach (var mapTile in m_mapTiles)
        {
            if (!mapTile)
            {
                return false;
            }
        }

        m_networkManagerFSM.AddOnEnterListener(DownloadMapTiles, NetworkManagerState.HostConnected);
        m_networkManagerFSM.AddOnEnterListener(DownloadMapTiles, NetworkManagerState.ClientConnected);

        return true;
    }

    protected virtual void Start()
    {
        if (!CheckPreconditions()) enabled = false;
    }

    private void DownloadMapTiles()
    {
        var swCorner = m_mapCenter + 0.5f * (Vector2.down + Vector2.left);
        swCorner = new Vector2(Mathf.Floor(swCorner.x), Mathf.Floor(swCorner.y));
        var nwCorner = swCorner + Vector2.right;
        var seCorner = swCorner + Vector2.up;
        var neCorner = swCorner + Vector2.right + Vector2.up;

        Vector2[] corners = { neCorner, seCorner, nwCorner, swCorner };

        for (var i = 0; i < m_mapTiles.Length; i++)
        {
            m_mapTiles[i].DownloadHeightmap(corners[i], m_serverPath);
        }
    }
}