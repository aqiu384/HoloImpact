using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;

/// <summary>
/// Downloads heightmap data and satellite imagery texture 
/// and loads them into the given terrain.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(Terrain))]
public class LoadMaptileData : NetworkBehaviour
{
    [SyncVar]
    public Vector3 maptileSize;
    [SyncVar]
    public int maptileResolution;
    [SyncVar]
    public string heightmapUrl;
    [SyncVar]
    public string imageTileUrl;
    [SyncVar]
    public int tileX;
    [SyncVar]
    public int tileY;

    [SerializeField]
    private Terrain m_terrain;
    [SerializeField]
    private TerrainData m_terrainData;

    protected virtual void Awake()
    {
        m_terrain = GetComponent<Terrain>();
        m_terrain.enabled = false;
    }

    public override void OnStartClient()
    {
        DownloadMaptileData();
    }

    public void DownloadMaptileData()
    {
        var terrainCollider = GetComponent<TerrainCollider>();
        m_terrainData = new TerrainData();

        m_terrain.transform.localPosition = new Vector3(maptileSize.x * (tileY - 0.5f), 0, maptileSize.z * -1 * (tileX + 0.5f));
        m_terrain.terrainData = m_terrainData;

        terrainCollider.terrainData = m_terrainData;
        terrainCollider.enabled = true;

        StartCoroutine(DownloadHeightmapCoroutine());
        StartCoroutine(DownloadImageTileCoroutine());
    }

    private IEnumerator DownloadImageTileCoroutine()
    {
        using (var www = new WWW(imageTileUrl))
        {
            yield return www;

            var splatPrototype = new SplatPrototype();
            splatPrototype.texture = www.texture;
            splatPrototype.tileSize = new Vector2(maptileSize.x, maptileSize.z);
            m_terrainData.splatPrototypes = new SplatPrototype[] { splatPrototype };
        }
    }

    private IEnumerator DownloadHeightmapCoroutine()
    {
        using (var www = new WWW(heightmapUrl))
        {
            yield return www;

            using (var reader = new BinaryReader(new MemoryStream(www.bytes)))
            {
                var resolution = maptileResolution + 1;
                var heights = new float[resolution, resolution];

                for (var x = 0; x < resolution; x++)
                {
                    for (var y = 0; y < resolution; y++)
                    {
                        heights[resolution - x - 1, y] = (float)reader.ReadUInt16() / 0xFFFF;
                    }
                }

                m_terrainData.heightmapResolution = resolution;
                m_terrainData.SetHeights(0, 0, heights);
                m_terrainData.size = maptileSize;

                m_terrain.enabled = true;
            }
        }
    }
}