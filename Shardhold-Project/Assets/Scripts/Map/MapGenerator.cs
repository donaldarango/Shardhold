using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static Card;

public class MapGenerator : MonoBehaviour
{
    public event EventHandler<SelectTileEventArgs> SelectTile;

    public int ringCount = 4; // Number of rings from the center base circle
    public int laneCount = 3; // Number of lanes per quadrant
    public float maxRadius = 6f;
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

    private int totalLaneCount;
    private float[] circleRadii;
    private float sectionAngle;
    private Dictionary<(int, int), MeshRenderer> tileMeshes = new Dictionary<(int, int), MeshRenderer>();

    private (int, int)? hoveredTile = null; // (ringNumber, laneNumber)
    private (int, int)? clickedTile = null; // (ringNumber, laneNumber)
	private static List<(int, int)> targetedTiles = null;
    void Start()
    {
        // Always set map at 0,0,0
        transform.position = Vector3.zero;

        totalLaneCount = laneCount * 4; 
        sectionAngle = 360f / totalLaneCount;
        circleRadii = new float[ringCount + 1];

		targetedTiles = new List<(int, int)>();

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
                    
                    if (clickedTile.HasValue) // There is a selected tile
                    {
                        if (clickedTile.Value == (r, l)) // If same tile is selected, deselect it
                        {
                            ResetTileColor(clickedTile.Value);
                            Debug.Log($"Deselected {clickedTile.Value}");
                            SelectTile?.Invoke(this, new SelectTileEventArgs(null));
                            clickedTile = null;
                        }
                        else // New tile selected
                        {
                            (int, int) prevTile = clickedTile.Value;
                            clickedTile = (r, l);
                            ResetTileColor(prevTile);
                            tileMeshes[(r, l)].material.color = clickColor;
                            SelectTile?.Invoke(this, new SelectTileEventArgs((r, l)));
                            Debug.Log($"Selected {clickedTile.Value}");
                        }
                    }
                    else // No currently selected tile
                    {
                        clickedTile = (r, l);
                        tileMeshes[(r, l)].material.color = clickColor;
                        SelectTile?.Invoke(this, new SelectTileEventArgs((r, l)));
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

public class SelectTileEventArgs
{
    public (int, int)? coords;
    public SelectTileEventArgs((int, int)? coords)
    {
        this.coords = coords;
    }
}
