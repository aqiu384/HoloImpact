using UnityEngine;

/// <summary>
/// Test for Plane Extruder utility.
/// May come in handy when displaying geographic zones on a map.
/// Generates a 4-pointed star prism.
/// </summary>
public class PolygonExtrusionTest : MonoBehaviour
{
    protected virtual void Awake()
    {
        var inputPoints = new float[,] {
            { 0, 3 },
            { 1, 1 },
            { 3, 0 },
            { 1, -1 },
            { 0, -3 },
            { -1, -1 },
            { -3, 0 },
            { -1, 1 }
        };

        var mesh = PolygonExtruder.CreatePolygon(inputPoints);
        PolygonExtruder.ExtrudePolygon(mesh, -1.0f, 1.0f);

        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Diffuse"));
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}