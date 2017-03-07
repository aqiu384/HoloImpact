using System;
using System.IO;
using System.Collections;
using UnityEngine;

/// <summary>
/// Updates terrain with data taken from the given tile server.
/// </summary>
[RequireComponent(typeof(Terrain))]
public class MapTile : MonoBehaviour
{
    public const int HEIGHTMAP_RESOLUTION = 1025;

    private Terrain m_terrain;
    private TerrainData m_terrainData;

    protected virtual void Awake()
    {
        m_terrain = GetComponent<Terrain>();
        m_terrainData = Instantiate(m_terrain.terrainData);
        m_terrainData.size = new Vector3(2.0f, 5.0f, 2.0f);
        m_terrain.terrainData = m_terrainData;
        

        var terrainCollider = GetComponent<TerrainCollider>();
        terrainCollider.terrainData = m_terrainData;
        terrainCollider.enabled = true;
    }

    public void DownloadHeightmap(Vector2 corner, string serverPath)
    {
        var heightmapFilepath = serverPath + LatlongToTileName(corner);
        StartCoroutine(DownloadHeightmapCoroutine(heightmapFilepath));
    }

    public static string LatlongToTileName(Vector2 latlong)
    {
        var bottom = Mathf.Floor(latlong.x);
        var right = Mathf.Floor(latlong.y);
        var lat = bottom < 0 ? "S" + (bottom * -1).ToString("N0") : "N" + bottom.ToString("N0");
        var lon = right < 0 ? "W" + (right * -1).ToString("N0") : "S" + right.ToString("N0");

        return lat + lon + "-1025.raw";
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