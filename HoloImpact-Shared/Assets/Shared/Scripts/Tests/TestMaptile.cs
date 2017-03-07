using System;
using System.IO;
using System.Collections;
using UnityEngine;

/// <summary>
/// Updates terrain with data taken from the given tile server.
/// </summary>
[RequireComponent(typeof(Terrain))]
public class TestMaptile : MonoBehaviour
{
    public Vector3 terrainSize = new Vector3(10.0f, 200.0f, 10.0f);
    public const int HEIGHTMAP_RESOLUTION = 256 + 1;

    private bool m_awake = false;
    private Terrain m_terrain;
    private TerrainData m_terrainData;

    private void OnValidate()
    {
        if (m_awake)
        {
            m_terrainData.size = terrainSize;

            if (m_terrainData.splatPrototypes.Length > 0)
            {
                Debug.Log("Hello");
                var splatPrototype = m_terrainData.splatPrototypes[0];
                // splatPrototype.texture = m_terrainData.splatPrototypes[0].texture;
                splatPrototype.tileSize = new Vector2(terrainSize.x, terrainSize.z);

                m_terrainData.splatPrototypes = new SplatPrototype[] { splatPrototype };
            }
        }
    }

    protected virtual void Awake()
    {
        m_terrain = GetComponent<Terrain>();
        m_terrainData = Instantiate(m_terrain.terrainData);
        m_terrainData.size = terrainSize;
        m_terrain.terrainData = m_terrainData;

        var terrainCollider = GetComponent<TerrainCollider>();
        terrainCollider.terrainData = m_terrainData;
        terrainCollider.enabled = true;

        // DownloadHeightmap("02301322121", "file:///C:/Users/aqiu/unity/HoloImpact-Shared/TileServer/RAW/");
        DownloadImageTile("02301322121", "file:///C:/Users/aqiu/unity/HoloImpact-Shared/TileServer/IMAGES/");

        m_awake = true;
    }

    public void DownloadImageTile(string quadKey, string serverPath)
    {
        var filepath = serverPath + quadKey + ".jpg";
        StartCoroutine(DownloadImageTileCoroutine(filepath));
    }

    private IEnumerator DownloadImageTileCoroutine(string filepath)
    {
        using (var www = new WWW(filepath))
        {
            yield return www;

            if (www.error != null)
            {
                throw new Exception("WWW failed to download from " + filepath + ":" + www.error);
            }

            var splatPrototype = new SplatPrototype();
            splatPrototype.texture = www.texture;
            splatPrototype.tileSize = new Vector2(10.0f, 10.0f);

            m_terrainData.splatPrototypes = new SplatPrototype[] { splatPrototype };
        }
    }

    public void DownloadHeightmap(string quadKey, string serverPath)
    {
        var heightmapFilepath = serverPath + quadKey + ".raw";
        StartCoroutine(DownloadHeightmapCoroutine(heightmapFilepath));
    }

    private IEnumerator DownloadHeightmapCoroutine(string heightmapFilepath)
    {
        using (var www = new WWW(heightmapFilepath))
        {
            yield return www;

            if (www.error != null)
            {
                throw new Exception("WWW failed to download from " + heightmapFilepath + ":" + www.error);
            }

            using (var reader = new BinaryReader(new MemoryStream(www.bytes)))
            {
                var heights = new float[HEIGHTMAP_RESOLUTION, HEIGHTMAP_RESOLUTION];

                for (var x = 0; x < HEIGHTMAP_RESOLUTION; x++)
                {
                    for (var y = 0; y < HEIGHTMAP_RESOLUTION; y++)
                    {
                        heights[HEIGHTMAP_RESOLUTION - x - 1, y] = (float)reader.ReadUInt16() / 0xFFFF;
                    }
                }

                var tileSize = m_terrain.terrainData.size;
                m_terrain.terrainData.heightmapResolution = HEIGHTMAP_RESOLUTION;
                m_terrain.terrainData.SetHeights(0, 0, heights);
                m_terrain.terrainData.size = tileSize;
            }
        }
    }
}