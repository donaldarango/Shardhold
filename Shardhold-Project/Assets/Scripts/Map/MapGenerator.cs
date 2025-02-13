using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapGenerator : MonoBehaviour
{
    public int circleCount = 5;
    public int sectionCount = 12;
    public float maxRadius = 5f;
    public Material defaultMaterial;
    public Color defaultColor = new Color(1f, 1f, 1f, 0.3f);
    public Color hoverColor = new Color(1f, 1f, 0.5f, 0.6f); // Yellowish hover effect
    public Color clickColor = new Color(0.8f, 0.3f, 0.3f, 0.6f); // Red when clicked
    // ADD MAP CONFIG (TERRAIN INFO)

    private float[] circleRadii;
    private float sectionAngle;
    private Dictionary<(int, int), MeshRenderer> tileMeshes = new Dictionary<(int, int), MeshRenderer>();
    private (int, int)? hoveredTile = null;
    private (int, int)? clickedTile = null;

    void Start()
    {
        sectionAngle = 360f / sectionCount;
        circleRadii = new float[circleCount];

        Assert.IsTrue(circleRadii.Length > 0);
        Assert.IsTrue(sectionCount % 4 == 0); // Sections must be divided into 4 directions

        for (int i = 0; i < circleCount; i++)
        {
            circleRadii[i] = ((i + 1) / (float)circleCount) * maxRadius;
        }

        MapManager.Instance.InitializeQuadrants();

        GenerateTiles();
        DrawCircles();
        //DrawRadialSections();
    }

    void Update()
    {
        HandleTileSelection();
    }

    // TODO: Optimize so that inner circle is either not drawn or does not have mesh collider
    //       Also change selected tile to have 0 indexed circle number (dont include inner circle)
    void GenerateTiles()
    {
        for (int c = 0; c < circleCount; c++)
        {
            float innerRadius = c == 0 ? 0f : circleRadii[c - 1];
            float outerRadius = circleRadii[c];

            for (int s = 0; s < sectionCount; s++)
            {
                int q = (int)(s / 3);

                float startAngle = (s * sectionAngle) * Mathf.Deg2Rad;
                float endAngle = ((s + 1) * sectionAngle) * Mathf.Deg2Rad;


                GameObject tileObj = new GameObject($"Tile_C{c}_S{s}");
                tileObj.transform.parent = transform;
                MeshFilter meshFilter = tileObj.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = tileObj.AddComponent<MeshRenderer>();

                // Do not add map tiles to first circle
                if (c != 0)
                {
                    
                    MapTile mapTile = new MapTile(c,s, new Vector2(0,0));
                    // TODO: Set terrain type
                    // TODO: Set map center coordinates of map tile

                    MapManager.Instance.AddTileToQuadrant(q, mapTile);
                }

                meshRenderer.material = new Material(defaultMaterial);
                meshRenderer.material.color = defaultColor;
                tileMeshes[(c, s)] = meshRenderer;

                MeshCollider meshCollider = tileObj.AddComponent<MeshCollider>();
                Mesh tileMesh = CreateCurvedTileMesh(innerRadius, outerRadius, startAngle, endAngle);
                meshFilter.mesh = tileMesh;
                meshCollider.sharedMesh = tileMesh;
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
        for (int i = 0; i < circleCount; i++)
        {
            float radius = circleRadii[i];
            LineRenderer circle = CreateLineRenderer($"Circle_{i}", Color.white);

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

    void DrawRadialSections()
    {
        if (circleCount < 1) return; // Ensure there's at least 1 circle

        float innerRadius = circleRadii[0]; // Start from circle 1

        for (int i = 0; i < sectionCount; i++)
        {
            float angle = i * sectionAngle * Mathf.Deg2Rad;
            Vector3 start = new Vector3(innerRadius * Mathf.Cos(angle), 0.01f, innerRadius * Mathf.Sin(angle)); // XZ plane
            Vector3 end = new Vector3(maxRadius * Mathf.Cos(angle), 0.01f, maxRadius * Mathf.Sin(angle));

            LineRenderer section = CreateLineRenderer($"Section_{i}", Color.white);
            section.positionCount = 2;
            section.SetPosition(0, start);
            section.SetPosition(1, end);
        }
    }

    LineRenderer CreateLineRenderer(string name, Color color)
    {
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
            if (tileName.StartsWith("Tile_C"))
            {
                string[] parts = tileName.Split('_');
                int c = int.Parse(parts[1].Substring(1));
                int s = int.Parse(parts[2].Substring(1));

                if (c != 0) // First circle not selectable
                {
                    if (hoveredTile.HasValue && hoveredTile.Value != (c, s))
                    {
                        ResetTileColor(hoveredTile.Value);
                    }

                    hoveredTile = (c, s);
                    tileMeshes[(c, s)].material.color = hoverColor;

                    // Handle mouse click
                    if (Input.GetMouseButtonDown(0)) // Left click
                    {
                        // Reset previously clicked tile if any
                        if (clickedTile.HasValue)
                        {
                            (int, int) prevTile = clickedTile.Value;
                            clickedTile = (c, s);
                            ResetTileColor(prevTile);
                        }
                        else
                        {
                            clickedTile = (c, s);
                        }
                        // Update clicked tile
                        tileMeshes[(c, s)].material.color = clickColor;
                        Debug.Log($"Selected {clickedTile.Value}");
                    }
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
