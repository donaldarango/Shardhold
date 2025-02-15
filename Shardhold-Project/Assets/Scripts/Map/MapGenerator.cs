using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Card;

public class MapGenerator : MonoBehaviour
{
    public int circleCount = 5;
    public int sectionCount = 12;
    public float maxRadius = 5f;
    public Material defaultMaterial;
    public Color defaultColor = new Color(1f, 1f, 1f, 0.3f);
    public Color hoverColor = new Color(1f, 1f, 0.5f, 0.6f); // Yellowish hover effect
    public Color clickColor = new Color(0.8f, 0.3f, 0.3f, 0.6f); // Red when clicked
    public Card selectedCard = null;

    public enum TargetType
    {
        Tile,
        Lane,
        Row,
        Quadrant,
        Ring,
        Board,
        Invalid
    }
    public TargetType type = TargetType.Invalid; //made public for testing purposes 
    public TargetType oldType = TargetType.Invalid;
    // ADD MAP CONFIG (TERRAIN INFO)

    private float[] circleRadii;
    private float sectionAngle;
    private Dictionary<(int, int), MeshRenderer> tileMeshes = new Dictionary<(int, int), MeshRenderer>();
    private (int, int)? hoveredTile = null;
    private (int, int)? clickedTile = null;
    //private static List<((int, int), MeshRenderer)> targetedTiles = null;
    private static List<(int, int)> targetedTiles = null;
    void Start()
    {
        sectionAngle = 360f / sectionCount;
        circleRadii = new float[circleCount];
        targetedTiles = new List<(int, int)>();

        for (int i = 0; i < circleCount; i++)
        {
            circleRadii[i] = ((i + 1) / (float)circleCount) * maxRadius;
        }

        GenerateTiles();
        DrawCircles();
        //DrawRadialSections();
    }

    void Update()
    {
        //if (selectedCard != null) //once card is implemented
        //{
        //    HandleTargeting(selectedCard.type);
        //}
        if (type != TargetType.Invalid)
        {
            HandleTargeting(type);
        }
        else
        {
            HandleTileSelection();
        }
    }

    void GenerateTiles()
    {
        for (int c = 0; c < circleCount; c++)
        {
            float innerRadius = c == 0 ? 0f : circleRadii[c - 1];
            float outerRadius = circleRadii[c];

            for (int s = 0; s < sectionCount; s++)
            {
                float startAngle = (s * sectionAngle) * Mathf.Deg2Rad;
                float endAngle = ((s + 1) * sectionAngle) * Mathf.Deg2Rad;

                GameObject tileObj = new GameObject($"Tile_C{c}_S{s}");
                tileObj.transform.parent = transform;
                MeshFilter meshFilter = tileObj.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = tileObj.AddComponent<MeshRenderer>();

                // Do not add map tiles to first circle
                if (c != 0)
                {
                    MapTile mapTile = tileObj.AddComponent<MapTile>();

                    mapTile.setCircleNumber(c);
                    mapTile.setSectorNumber(s);
                    // TODO: Set terrain type
                }

                meshRenderer.material = new Material(defaultMaterial);
                meshRenderer.material.color = defaultColor;
                tileMeshes[(c, s)] = meshRenderer;

                MeshCollider meshCollider = tileObj.AddComponent<MeshCollider>();
                Mesh tileMesh = CreateCurvedTileMesh(innerRadius, outerRadius, startAngle, endAngle);
                meshFilter.mesh = tileMesh;
                meshCollider.sharedMesh = tileMesh;

                //AddTileBorder(tileObj, innerRadius, outerRadius, startAngle, endAngle);
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

    public void HandleTargeting(TargetType type) // in my head this would pull from the current card from the hand class
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

                    if(hoveredTile.HasValue && !targetedTiles.Contains((c,s)))
                    {
                        foreach(var tile in targetedTiles)
                        {
                            
                            ResetTileColor(tile);
                        }
                        targetedTiles.Clear();
                        //ResetTileColor(hoveredTile.Value);
                    }

                    hoveredTile = (c, s);
                    switch (type)
                    {
                        case TargetType.Tile:

                            AppendTile((c, s));

                            //tileMeshes[(c, s)].material.color = hoverColor;
                            //targetedTiles.Add(tileMeshes[(c, s)]);
                            break; 

                        case TargetType.Lane:
                            
                            for (int i = 1; i < 5; ++i)
                            {
                                AppendTile((i, s));
                                //tileMeshes[(i, s)].material.color = hoverColor;
                                //targetedTiles.Add(tileMeshes[(i, s)]);
                            }
                            break;

                        case TargetType.Row:

                            AppendTile((c, s));
                            AppendTile((c, (s - 1) % 12));
                            AppendTile((c, (s + 1) % 12));

                            //tileMeshes[(c, s)].material.color = hoverColor;
                            //tileMeshes[(c, (s - 1) % 12)].material.color = hoverColor;
                            //tileMeshes[(c, (s + 1) % 12)].material.color = hoverColor;
                            //
                            //targetedTiles.Add(tileMeshes[(c, s)]);
                            //targetedTiles.Add(tileMeshes[((c, (s - 1) % 12)]);
                            //targetedTiles.Add(tileMeshes[(c, (s + 1) % 12)]);
                            break;

                        case TargetType.Quadrant:
                            
                            for (int i = 1; i < 5; ++i)
                            {
                                AppendTile((c, s));
                                AppendTile((c, (s - 1) % 12));
                                AppendTile((c, (s + 1) % 12));
                                //tileMeshes[(i, s)].material.color = hoverColor;
                                //tileMeshes[(i, (s - 1) % 12)].material.color = hoverColor;
                                //tileMeshes[(i, (s + 1) % 12)].material.color = hoverColor;
                            }
                            break;

                        case TargetType.Ring:

                            for (int i = 0; i < 13; ++i)
                            {
                                AppendTile((c, i));
                                //tileMeshes[(c, i)].material.color = hoverColor;
                            }
                            break;

                        case TargetType.Board:
                            for (int i = 1; i < 5; ++i)
                            {
                                for (int j = 0; j < 13; ++j)
                                {
                                    AppendTile((i, j));
                                    //tileMeshes[(i, j)].material.color = hoverColor;
                                }
                            }

                            break;

                        default:
                            break;

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

    void AppendTile((int, int) tile)
    {
        tileMeshes[tile].material.color = hoverColor;
        targetedTiles.Add(tile);
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
