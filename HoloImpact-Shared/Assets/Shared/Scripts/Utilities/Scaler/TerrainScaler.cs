using UnityEngine;

/// <summary>
/// Required to scale terrain size since normal Unity transforms do not affect terrains.
/// </summary>
[RequireComponent(typeof(Terrain))]
public class TerrainScaler : Scaler
{
    [SerializeField]
    Terrain m_terrain;

    protected virtual void Awake()
    {
        m_terrain = GetComponent<Terrain>();
    }

    public override void ScaleBy(Vector3 scale)
    {
        var newSize = Vector3.Scale(m_terrain.terrainData.size, scale);

        m_terrain.terrainData.size = newSize;

        var splatPrototype = m_terrain.terrainData.splatPrototypes[0];
        splatPrototype.tileSize = new Vector2(newSize.x, newSize.z);

        m_terrain.terrainData.splatPrototypes = new SplatPrototype[] { splatPrototype };
    }
}