﻿using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Spawns a grid of map tiles centered around the given coordinate.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class MaptileSpawner : MonoBehaviour
{
    public GameObject maptilePrefab;
    public Transform targetSpawnArea;
    public Vector3 mapCenter = new Vector3(-117.1611f, 0, 32.7157f);
    public Vector3 maptileSize = new Vector3(10.0f, 200.0f, 10.0f);
    public MaptileGridSize mapGridSize = MaptileGridSize.OneByOne;
    public bool useLocalTileServer = true;

    public MaptileImportConfiguration m_maptileImportConfig;

    private int m_mapPixelCenterX, m_mapPixelCenterY;

    private bool m_stopThread;
    private Thread m_thread;
    private object m_lock = new object();
    private Queue<string> m_quadKeys = new Queue<string>();

    private MaptileServer maptileServer;

    protected virtual void Awake()
    {
        GetComponent<NetworkIdentity>().serverOnly = true;
    }

    protected virtual void OnEnable()
    {
        if (useLocalTileServer)
        {
            m_maptileImportConfig.maptileServerAddress = "file:///" + m_maptileImportConfig.maptileDirectory;
        }
        else
        {
            maptileServer = new MaptileServer(m_maptileImportConfig);
            maptileServer.StartServer();
        }

        StartCoroutine(SpawnMaptilesCoroutine());
    }

    protected virtual void OnDisable()
    {
        if (maptileServer != null)
        {
            maptileServer.StopServer();
            maptileServer = null;
        }

        StopAllCoroutines();
    }

    private void SpawnMaptile(int tileX, int tileY, string quadKey)
    {
        var loadTerrainData = maptilePrefab.GetComponent<LoadMaptileData>();

        loadTerrainData.gameObject.name = quadKey;
        loadTerrainData.maptileSize = maptileSize;
        loadTerrainData.maptileResolution = m_maptileImportConfig.outputTileSize;

        loadTerrainData.heightmapUrl = m_maptileImportConfig.maptileServerAddress + "/" + m_maptileImportConfig.GetWebFilePath(MaptileWorkingDirectory.Heightmap, quadKey);
        loadTerrainData.imageTileUrl = m_maptileImportConfig.maptileServerAddress + "/" + m_maptileImportConfig.GetWebFilePath(MaptileWorkingDirectory.Image, quadKey);

        loadTerrainData.tileX = tileX;
        loadTerrainData.tileY = tileY;

        var spawnedObject = Instantiate(loadTerrainData.gameObject);
        NetworkServer.Spawn(spawnedObject);

        spawnedObject.GetComponent<MyNetworkTransform>().SetParent(targetSpawnArea, false);
    }

    private IEnumerator SpawnMaptilesCoroutine()
    {
        var gridOffset = (int)mapGridSize;
        var gridWidth = (1 + 2 * gridOffset);
        StartThread();

        string quadKey;
        var index = 0;

        while (index < gridWidth * gridWidth)
        {
            if (m_quadKeys.Count > 0)
            {
                lock (m_lock) quadKey = m_quadKeys.Dequeue();
                SpawnMaptile(index % gridWidth - gridOffset, index / gridWidth - gridOffset, quadKey);
                index++;
            }

            yield return null;
        }

        StopThread();
    }

    private void StartThread()
    {
        if (m_thread == null)
        {
            lock (m_lock) m_stopThread = false;
            m_thread = new Thread(GetMaptileQuadkeysThread);
            m_thread.Start();
        }
    }

    private void StopThread()
    {
        if (m_thread != null)
        {
            lock (m_lock) m_stopThread = true;
            m_thread.Join();
            m_thread = null;
        }
    }

    private void GetMaptileQuadkeysThread()
    {
        var maptileImporter = new MaptileImporter(m_maptileImportConfig);
        foreach (var quadKey in maptileImporter.GetMaptileQuadkeys(mapCenter.z, mapCenter.x, mapGridSize))
        {
            lock (m_lock) if (m_stopThread) break;
            lock (m_lock) m_quadKeys.Enqueue(quadKey);
        }
    }
}