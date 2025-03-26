using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static Card;
#nullable enable 

public class MapGenerator : MonoBehaviour
{
    public delegate void HoverEventHandler(TileActor ta);
    public static event HoverEventHandler HoverTile;
    public static event EventHandler<SelectTileEventArgs> SelectTile;
    public static event EventHandler<SelectTileSetEventArgs> SelectTileSet;

    public int ringCount = 4; // Number of rings from the center base circle
    public int laneCount = 3; // Number of lanes per quadrant
    public float maxRadius = 6f;
    public Material lineMaterial;
    public Color defaultColor = new Color(1f, 1f, 1f, 1.0f);
    public Color hoverColor = new Color(1f, 1f, 0.5f, 0.6f); // Yellowish hover effect
    public Color clickColor = new Color(0.8f, 0.3f, 0.3f, 0.6f); // Red when clicked
    public Color farColor = new Color(0.2f, 0.6f, 0.5f); //Bluish color when out of range
    public List<TerrainSO> terrainConfigs = new List<TerrainSO>();

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

    public AllyUnit? selectedUnit = null; 
    public Card? selectedCard = null;
    public Card? oldCard = null;
    public delegate void PlayCardHandler(HashSet<(int, int)> tiles);
    public static event PlayCardHandler PlayCard;

    // ADD MAP CONFIG (TERRAIN INFO)

    private int totalLaneCount;
    private float[] circleRadii;
    private float sectionAngle;
    private Dictionary<(int, int), MeshRenderer> tileMeshes = new Dictionary<(int, int), MeshRenderer>();

    private (int, int)? hoveredTile = null; // (ringNumber, laneNumber)
    private (int, int)? clickedTile = null; // (ringNumber, laneNumber)
	//private static List<(int, int)> targetedTiles = null;

    private static HashSet<(int, int)> targetedTiles = null;
    private static HashSet<(int, int)> clickedTiles = null;

    private void OnEnable()
    {
        TileActorManager.EndEnemyTurn += OnRoundResetSelection;
    }

    private void OnDisable()
    {
        TileActorManager.EndEnemyTurn -= OnRoundResetSelection;

    }

    void Start()
    {
        // Always set map at 0,0,0
        transform.position = Vector3.zero;

        totalLaneCount = laneCount * 4; 
        sectionAngle = 360f / totalLaneCount;
        circleRadii = new float[ringCount + 1];

		targetedTiles = new HashSet<(int, int)>();
        clickedTiles = new HashSet<(int, int)>();
        

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
        if (selectedCard != null || selectedUnit != null)
        {
            HandleTargeting(selectedCard, selectedUnit);
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

                // Determine Terrain Type and update materials
                // TODO: read in map terrain info

                // TESTING: even quadrant = default ; odd = mountain
                Terrain terrain;
                if (q % 2 == 0)
                {
                    terrain = CreateTerrain(TerrainType.Default);
                }
                else
                {
                    terrain = CreateTerrain(TerrainType.Mountain);
                }
                SetMeshRendererTerrainMaterial(terrain, meshRenderer);

                MapTile mapTile = new MapTile((Quadrant)q, r, l, tileCenter, terrain);
                MapManager.Instance.AddTileToQuadrant(q, mapTile);
            }
        }
    }

    Mesh CreateCurvedTileMesh(float innerRadius, float outerRadius, float startAngle, float endAngle)
    {
        int segments = 10;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            vertices.Add(new Vector3(innerRadius * Mathf.Cos(angle), 0, innerRadius * Mathf.Sin(angle)));
            vertices.Add(new Vector3(outerRadius * Mathf.Cos(angle), 0, outerRadius * Mathf.Sin(angle)));
            float u = t;

            uvs.Add(new Vector2(u, 0));
            uvs.Add(new Vector2(u, 1));
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
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
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
        lr.material = lineMaterial;
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

                // Quadrant check and also debugging messages to check for tileactor
                MapTile tile = MapManager.Instance.GetTile(r, l);
                HoverTile?.Invoke(tile.GetCurrentTileActor());


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

    void HandleTargeting(Card? card = null, AllyUnit? unit = null)
    {
        if (card != oldCard) // If a new selection type was just picked, clear the board for highlights and clicks. This would happen on swapping cards.
        {
            clickedTile = null;
            clickedTiles.Clear();
            targetedTiles.Clear();
            oldCard = card;

            foreach(var tile in tileMeshes)
            {
                ResetTileColor(tile.Key);// tile.Value.material.color = defaultColor;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            string tileName = hit.collider.gameObject.name;
            if (tileName.StartsWith("Tile_R"))
            {
                int range = card != null ? card.range : (unit != null ? unit.stats.range : 0);
                string[] parts = tileName.Split('_');
                int r = int.Parse(parts[1].Substring(1));
                int l = int.Parse(parts[2].Substring(1));

                // If set of hovered tiles doesn't include this new tile, then we need to make a new highlight
                if (!targetedTiles.Contains((r, l))) // We can do this because given quadrant locking, each tile is in one unique selection per targeting type
                {
                    foreach (var tile in targetedTiles)
                    {
                        ResetTileColor(tile);
                    }
                    targetedTiles.Clear();
                }

                if (r > (range - 1)) // If hovering on a tile out of range, don't show a highlight. It's not a valid target
                {
                    return;
                }

                int offset = Mathf.FloorToInt(l / 3) * 3; // Cull targeting into either 0, 3, 6, or 9 so quadrants are respected
                switch (card != null ? card.targetType : (unit != null ? unit.stats.targetType : TargetType.Invalid))
                {
                    case TargetType.Tile: // Single tile, like the prior implementation

                        AppendTile((r, l));
                        break;

                    case TargetType.Lane: // Entire column of tiles

                        for (int i = 0; i < Math.Min(range, 4); ++i)
                        {
                            AppendTile((i, l));
                        }
                        break;

                    case TargetType.Row: // Quarter of a ring

                        AppendTile((r, offset));
                        AppendTile((r, offset + 1));
                        AppendTile((r, offset + 2));
                        break;

                    case TargetType.Quadrant: // Quarter of the board

                        for (int i = 0; i < Math.Min(range, 4); ++i)
                        {
                            AppendTile((i, offset));
                            AppendTile((i, offset + 1));
                            AppendTile((i, offset + 2));
                        }
                        break;

                    case TargetType.Ring: // Full circle of tiles

                        for (int i = 0; i < 12; ++i)
                        {
                            AppendTile((r, i));
                        }
                        break;

                    case TargetType.Board: // Every tile
                        for (int i = 0; i < Math.Min(range, 4); ++i)
                        {
                            for (int j = 0; j < 12; ++j)
                            {
                                AppendTile((i, j));
                            }
                        }
                        break;

                    default:
                        break;

                }

                // Handle mouse click
                if (Input.GetMouseButtonDown(0)) // Left click
                {
                    if(clickedTiles.Count > 0) // If there are already some tiles clicked,
                    {
                        if (clickedTiles.Contains((r, l))){ //...remove the red highlight since we reselected the same section.
                            Debug.Log("Delete Target");
                            var temp = clickedTiles;
                            clickedTiles.Clear();
                            foreach (var tile in temp)
                            {
                                tileMeshes[tile].material.color = defaultColor;
                            }
                            //clickedTiles.Clear();
                            targetedTiles.Clear();
                            //PlayCard?.Invoke(null);
                            //selectedCard = null;
                        }
                        else //...remove the old red highlight and make a new one since we made a new selection.
                        {
                            Debug.Log("New Target - tile: " + (r, l));
                            foreach (var tile in clickedTiles)
                            {
                                tileMeshes[tile].material.color = defaultColor;
                            }
                            clickedTiles.Clear();


                            foreach (var tile in targetedTiles)
                            {
                                clickedTiles.Add(tile);
                                tileMeshes[tile].material.color = clickColor;
                            }
                            targetedTiles.Clear();
                            Debug.Log("play card via new target");
                            if (selectedCard != null)
                            {
                                selectedCard.Play(clickedTiles);
                                selectedCard = null;
                                StartCoroutine(RemoveHighlightDelayed(clickedTiles));
                            }
                            else if(selectedUnit != null)
                            {
                                selectedUnit.Play(clickedTiles);
                                selectedUnit = null;
                                StartCoroutine(RemoveHighlightDelayed(clickedTiles));
                            }
                            //PlayCard?.Invoke(clickedTiles);
                            //selectedCard = null;

                        }
                    }
                    else // If there isn't a red highlight, this is the first selection.
                    {
                        Debug.Log("First Target - tile: " + (r, l));
                        foreach (var tile in targetedTiles)
                        {
                            tileMeshes[tile].material.color = clickColor;
                            clickedTiles.Add(tile);
                        }
                        targetedTiles.Clear();
                        Debug.Log("play card via first target");

                        if (selectedCard != null)
                        {
                            selectedCard.Play(clickedTiles);
                            selectedCard = null;
                            StartCoroutine(RemoveHighlightDelayed(clickedTiles));
                        }
                        else if (selectedUnit != null)
                        {
                            selectedUnit.Play(clickedTiles);
                            selectedUnit = null;
                            StartCoroutine(RemoveHighlightDelayed(clickedTiles));
                        }
                        //PlayCard?.Invoke(clickedTiles);
                    }
                }
            }
            else if (tileName.Contains("Base"))
            {
                if (Input.GetMouseButtonDown(0)) // Left click
                {
                    if (card != null && selectedCard != null && card.targetType == TargetType.Tile && card.cardType == CardType.Spell)
                    {
                        HashSet<(int, int)> set = new HashSet<(int, int)>();
                        set.Add((-1, -1));
                        selectedCard.Play(set);
                        selectedCard = null;
                        StartCoroutine(RemoveHighlightDelayed(clickedTiles));
                    }
                }
            }
        }
        else
        {
            if (targetedTiles.Count > 0) //Reset group hover if the mouse is off the board
            {
                foreach (var tile in targetedTiles)
                {
                    ResetTileColor(tile);
                }
                targetedTiles.Clear();
            }
        }
    }

    IEnumerator RemoveHighlightDelayed(HashSet<(int, int)> set)
    {
        foreach (var tile in tileMeshes)
        {
            ResetTileColor(tile.Key);// tile.Value.material.color = defaultColor;
        }
        yield return new WaitForSeconds(.5f);
        clickedTiles.Clear();
        foreach (var tile in tileMeshes)
        {
            ResetTileColor(tile.Key);// tile.Value.material.color = defaultColor;
        }
    }

    void AppendTile((int, int) tile)
    {
        tileMeshes[tile].material.color = hoverColor;
        targetedTiles.Add(tile);
    }


    void ResetTileColor((int, int) tile)
    {
        
        if ((clickedTile.HasValue && clickedTile.Value == tile) || (clickedTiles.Count > 0 && clickedTiles.Contains(tile)))
        {
            // Keep clicked tile color as clickColor
            tileMeshes[tile].material.color = clickColor;
        }
        else if ((selectedCard != null || selectedUnit != null) && tile.Item1 >= (selectedCard != null ? selectedCard.range : selectedUnit != null ? selectedUnit.stats.range : 0))
        {   
            // Tile is out of range of current card, set it to farColor
            tileMeshes[tile].material.color = farColor;
        }
        else
        {
            // Reset other tile to default color
            tileMeshes[tile].material.color = defaultColor;
        }
    
    }

    public Terrain CreateTerrain(TerrainType terrainType)
    {
        TerrainSO terrainData;
        for (int i = 0; i < terrainConfigs.Count; i++)
        {
            terrainData = terrainConfigs[i];
            if (terrainData.terrainType == terrainType)
            {
                switch (terrainType)
                {
                    case TerrainType.Default:
                        return new DefaultTerrain(terrainData);
                    case TerrainType.Mountain:
                        return new MountainTerrain(terrainData);
                    default:
                        throw new Exception("TerrainType needs to be added to CreateTerrain switch case");
                }
            }
        }
        throw new Exception("Terraintype needs to be added to config list");
    }

    public void SetMeshRendererTerrainMaterial(Terrain terrain, MeshRenderer meshRenderer)
    {
        meshRenderer.material = new Material(terrain.terrainMaterial);
        meshRenderer.material.color = defaultColor;
        meshRenderer.material.mainTextureScale = new Vector2(1, 1);
    }

    public void SelectCard(Card newCard)
    {
        selectedCard = newCard != null ? newCard : null;
        selectedUnit = null;

        clickedTile = null;
        clickedTiles.Clear();
        targetedTiles.Clear();

        HandleTargeting(selectedCard);

        foreach (var tile in tileMeshes)
        {
            ResetTileColor(tile.Key);
        }
    }
    public void SelectUnit(AllyUnit newUnit)
    {
        selectedUnit = newUnit != null ? newUnit : null;
        selectedCard = null;

        clickedTile = null;
        clickedTiles.Clear();
        targetedTiles.Clear();

        HandleTargeting(null, newUnit);

        foreach (var tile in tileMeshes)
        {
            ResetTileColor(tile.Key);
        }
    }

    private void OnRoundResetSelection()
    {
        clickedTile = null;
        clickedTiles.Clear();
        targetedTiles.Clear();

        foreach (var tile in tileMeshes)
        {
            ResetTileColor(tile.Key);
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

public class SelectTileSetEventArgs //just to differentiate & give it all the relevant tiles
{
    public HashSet<(int, int)> coordSet;
    public SelectTileSetEventArgs(HashSet<(int, int)> coords)
    {
        this.coordSet = coords;

    }
}
