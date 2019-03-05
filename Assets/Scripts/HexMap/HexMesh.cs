using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    MeshCollider collider;

    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        collider = GetComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }


    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();

        collider.sharedMesh = hexMesh;
    }


    void Triangulate(HexCell cell)
    {
        for (HexDirections d = HexDirections.NE; d <= HexDirections.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    void Triangulate(HexDirections direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;

        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        
        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.color);

        if(direction <= HexDirections.SE)
        {
            TriangulateConnection(cell, direction, v1, v2);
        }
    }

    void TriangulateConnection(HexCell cell, HexDirections direction, Vector3 v1, Vector3 v2)
    {
        HexCell neighbor = cell.GetNeighbors(direction);
        if (neighbor == null)
            return;

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;

        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;
        
        TriangulateConnectionTerrace(v1, v2, cell, v3, v4, neighbor);
        
        // Add triagles betwen bridges.
        HexCell nextNeighbor = cell.GetNeighbors(direction.Next());
        if (direction <= HexDirections.E && nextNeighbor != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;

            AddTriangle(v2, v4, v5);
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }
    }

    public void TriangulateConnectionTerrace(Vector3 startLeft, Vector3 startRight, HexCell startCell,
        Vector3 endLeft, Vector3 endRight, HexCell endCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(startLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(startRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(startCell.color, endCell.color, 1);

        AddQuad(startLeft, startRight, v3, v4);

        AddQuadColor(
            startCell.color,
            c2
            );

        for(int i = 2; i <= HexMetrics.terracesSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(startLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(startRight, endRight, i);
            c2 = HexMetrics.TerraceLerp(startCell.color, endCell.color, i);

            AddQuad(v1, v2, v3, v4);

            AddQuadColor(c1, c2);
        }
    }

    /// <summary>
    /// Add new triangle to mesh.
    /// </summary>
    /// <param name="v1">First corner</param>
    /// <param name="v2">Second corner</param>
    /// <param name="v3">Third corner</param>
    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    /// <summary>
    /// Add new triangle solid color to mesh. 
    /// Use after AddTriangle function call only.
    /// </summary>
    public void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    /// <summary>
    /// Add new color to each corner of triangle to mesh. 
    /// Use after AddTriangle function call only.
    /// </summary>
    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    /// <summary>
    /// Add new quad to mesh.
    /// </summary>
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    /// <summary>
    /// Add new color to each side of quad. 
    /// Use after AddQuad function only.
    /// </summary>
    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
