using System;
using UnityEngine;

public class PolygonExtruder
{
    /// <summary>
    /// Create polygon mesh from list of perimeter points.
    /// </summary>
    /// <param name="points">List of points along perimeter</param>
    /// <returns></returns>
    public static Mesh CreatePolygon(float[,] points)
    {
        var vertices2d = new Vector2[points.GetLength(0)];
        var vertices = new Vector3[points.GetLength(0)];

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices2d[i] = new Vector2(points[i, 0], points[i, 1]);
            vertices[i] = new Vector3(points[i, 0], 0, points[i, 1]);
        }

        var triangulator = new Triangulator(vertices2d);
        var triangles = triangulator.Triangulate();

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary>
    /// Extrude polygon plane into a prism.
    /// </summary>
    /// <param name="mesh">Prism caps</param>
    /// <param name="bottomDistance">Height from original polygon to extruded base</param>
    /// <param name="topDistance">Height from original polygon to extruded top</param>
    public static void ExtrudePolygon(Mesh mesh, float bottomDistance, float topDistance)
    {
        var topOffset = Vector3.up * topDistance;
        var bottomOffset = Vector3.up * bottomDistance;

        var vertices = mesh.vertices;
        var triangles = mesh.triangles;

        var verticesLen = vertices.Length;
        var trianglesLen = triangles.Length;

        var vertices2x = new Vector3[2 * verticesLen];
        var triangles2x = new int[2 * trianglesLen + 6 * verticesLen];
        var sideTriangles = new int[6 * verticesLen];

        for (var i = 0; i < verticesLen; i++)
        {
            vertices2x[i] = vertices[i] + topOffset;
            vertices2x[verticesLen + i] = vertices[i] + bottomOffset;
        }

        for (var i = 0; i < trianglesLen; i++)
        {
            triangles2x[i] = triangles[i];
            triangles2x[2 * trianglesLen - 1 - i] = triangles[i] + verticesLen;
        }

        var windingDirection =
            Math.Sign(triangles[0] - triangles[2]) +
            Math.Sign(triangles[1] - triangles[0]) +
            Math.Sign(triangles[2] - triangles[1]);

        if (windingDirection < 0)
        {
            for (var i = 0; i < verticesLen; i++)
            {
                sideTriangles[6 * i] = i;
                sideTriangles[6 * i + 1] = i + 1;
                sideTriangles[6 * i + 2] = verticesLen + i;

                sideTriangles[6 * i + 3] = verticesLen + i;
                sideTriangles[6 * i + 4] = i + 1;
                sideTriangles[6 * i + 5] = verticesLen + i + 1;
            }

            sideTriangles[6 * verticesLen - 5] = 0;
            sideTriangles[6 * verticesLen - 2] = 0;
            sideTriangles[6 * verticesLen - 1] = verticesLen;
        }
        else
        {
            for (var i = 0; i < verticesLen; i++)
            {
                sideTriangles[6 * i] = i;
                sideTriangles[6 * i + 1] = verticesLen + i;
                sideTriangles[6 * i + 2] = i + 1;

                sideTriangles[6 * i + 3] = i + 1;
                sideTriangles[6 * i + 4] = verticesLen + i;
                sideTriangles[6 * i + 5] = verticesLen + i + 1;
            }

            sideTriangles[6 * verticesLen - 4] = 0;
            sideTriangles[6 * verticesLen - 3] = 0;
            sideTriangles[6 * verticesLen - 1] = verticesLen;
        }

        sideTriangles.CopyTo(triangles2x, 2 * trianglesLen);

        mesh.vertices = vertices2x;
        mesh.triangles = triangles2x;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}