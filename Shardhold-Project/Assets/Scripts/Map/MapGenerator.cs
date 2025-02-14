using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapGenerator : MonoBehaviour
{
    public int ringCount = 4; // Number of rings from the center base circle
    public int laneCount = 3; // Number of lanes per quadrant
    public float maxRadius = 6f;
    public Material defaultMaterial;
    public Color defaultColor = new Color(1f, 1f, 1f, 0.3f);
    public Color hoverColor = new Color(1f, 1f, 0.5f, 0.6f); // Yellowish hover effect
    public Color clickColor = new Color(0.8f, 0.3f, 0.3f, 0.6f); // Red when clicked
    // ADD MAP CONFIG (TERRAIN INFO)

    private int totalLaneCount;
    private float[] circleRadii;
    private float sectionAngle;
    private Dictionary<(int, int), MeshRenderer> tileMeshes = new Dictionary<(int, int), MeshRenderer>();
    private (int, int)? hoveredTile = null;
    private (int, int)? clickedTile = null;

    void Start()
    {
        // Always set map at 0,0,0
        transform.position = Vector3.zero;

        totalLaneCount = laneCount * 4; 
        sectionAngle = 360f / totalLaneCount;
        circleRadii = new float[ringCount + 1];

        // Debugging
        Assert.IsTrue(circleRadii.Length > 0);
        Assert.IsTrue(laneCount > 0);
        Assert.IsTrue(ringCount > 0);

        for (int i = 0; i < ringCount + 1; i++)
        {
            circleRadii[i] = ((i + 1) / (float)(ringCount + 1)) * (maxRadius);
        }

        MapManager.Instance.SetLaneCount(laneCount);
        MapManager.Instance.SetRingCount(ringCount);
        MapManager.Instance.InitializeQuadrants();

        GenerateTiles();
        DrawCircles();
    }

    void Update()
    {
        HandleTileSelection();
    }

    void GenerateTiles()
    {
        for (int r = 0; r < ringCount; r++)
        {
            float innerRadius = circleRadii[r];
            float outerRadius = circleRadii[r + 1];

            for (int l = 0; l < totalLaneCount; l++)
            {
                int q = (int)(l / 3);

                float startAngle = (l * sectionAngle) * Mathf.Deg2Rad;
                float endAngle = ((l + 1) * sectionAngle) * Mathf.Deg2Rad;

                GameObject tileObj = new GameObject($"Tile_R{r}_L{l}");
                tileObj.transform.parent = transform;
                MeshFilter meshFilter = tileObj.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = tileObj.AddComponent<MeshRenderer>();

                meshRenderer.material = new Material(defaultMaterial);
                meshRenderer.material.color = defaultColor;
                tileMeshes[(r, l)] = meshRenderer;

                MeshCollider meshCollider = tileObj.AddComponent<MeshCollider>();
                Mesh tileMesh = CreateCurvedTileMesh(innerRadius, outerRadius, startAngle, endAngle);
                meshFilter.mesh = tileMesh;
                meshCollider.sharedMesh = tileMesh;

                // Calculate center position of tile
                float centerDist = (innerRadius + outerRadius) / 2.0f;
                float centerRad = (startAngle + endAngle) / 2.0f;
                float x = Mathf.Cos(centerRad);
                float z = Mathf.Sin(centerRad);
                Vector3 tileCenter = new Vector3(x, 0.0f, z) * centerDist;

                MapTile mapTile = new MapTile(r, l, tileCenter);
                // TODO: Set terrain type

                MapManager.Instance.AddTileToQuadrant(q, mapTile);

                // Add enemy to every tile to test tile centers
                MapManager.Instance.AddEnemyToTile(q, r, l, 0);
            }
        }
    }

    Mesh CreateCurvedTileMesh(float innerRadius, float outerRadius, float startAngle, float endAngle)
    {
        int segments = 10;
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            vertices.Add(new Vector3(innerRadius * Mathf.Cos(angle), 0, innerRadius * Mathf.Sin(angle)));
            vertices.Add(new Vector3(outerRadius * Mathf.Cos(angle), 0, outerRadius * Mathf.Sin(angle)));
        }

        List<int> triangles = new List<int>();

        for (int i = 0; i < segments; i++)
        {
            int a = i * 2;
            int b = a + 1;
            int c = a + 2;
            int d = a + 3;

            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);

            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(d);
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.RecalculateNormals();
        return mesh;
    }


    void DrawCircles()
    {
        for (int i = 0; i < ringCount + 1; i++)
        {
            float radius = circleRadii[i];
            LineRenderer circle = CreateLineRenderer($"Circle_{i}", Color.black);

            int segments = 50; // Smoothness of circles
            Vector3[] points = new Vector3[segments + 1];

            for (int j = 0; j <= segments; j++)
            {
                float angle = (j / (float)segments) * 2 * Mathf.PI;
                points[j] = new Vector3(radius * Mathf.Cos(angle), 0.01f, radius * Mathf.Sin(angle)); // XZ plane
            }

            circle.positionCount = points.Length;
            circle.SetPositions(points);
        }
    }

    LineRenderer CreateLineRenderer(string name, Color color)
    {
        // TODO: Fix Line coloring
        GameObject obj = new GameObject(name);
        obj.transform.parent = transform;
        LineRenderer lr = obj.AddComponent<LineRenderer>();

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = defaultMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = false;
        return lr;
    }

    void HandleTileSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            string tileName = hit.collider.gameObject.name;
            if (tileName.StartsWith("Tile_R"))
            {
                string[] parts = tileName.Split('_');
                int r = int.Parse(parts[1].Substring(1));
                int l = int.Parse(parts[2].Substring(1));

                
                if (hoveredTile.HasValue && hoveredTile.Value != (r, l))
                {
                    ResetTileColor(hoveredTile.Value);
                }

                hoveredTile = (r, l);
                tileMeshes[(r, l)].material.color = hoverColor;

                // Handle mouse click
                if (Input.GetMouseButtonDown(0)) // Left click
                {
                    // Reset previously clicked tile if any
                    if (clickedTile.HasValue)
                    {
                        (int, int) prevTile = clickedTile.Value;
                        clickedTile = (r, l);
                        ResetTileColor(prevTile);
                    }
                    else
                    {
                        clickedTile = (r, l);
                    }
                    // Update clicked tile
                    tileMeshes[(r, l)].material.color = clickColor;
                    Debug.Log($"Selected {clickedTile.Value}");
                }
            }
        }
        else
        {
            // Reset hover if no tile is hit
            if (hoveredTile.HasValue)
            {
                ResetTileColor(hoveredTile.Value);
                hoveredTile = null;
            }
        }
    }

    void ResetTileColor((int, int) tile)
    {
        if (clickedTile.HasValue && clickedTile.Value == tile)
        {
            // Keep clicked tile color as clickColor
            tileMeshes[tile].material.color = clickColor;
        }
        else
        {
            // Reset other tile to default color
            tileMeshes[tile].material.color = defaultColor;
        }
    }
}
