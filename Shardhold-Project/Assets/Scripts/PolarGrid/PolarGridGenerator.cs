using UnityEngine;

public class PolarGridGenerator : MonoBehaviour
{
    public int circleCount = 5; // Number of concentric circles
    public int sectionCount = 8; // Number of radial divisions
    public float maxRadius = 5f; // Maximum radius of the grid
    public Material lineMaterial; // Material for grid lines

    private float[] circleRadii;
    private float sectionAngle;

    void Start()
    {
        sectionAngle = 360f / sectionCount;
        circleRadii = new float[circleCount];

        for (int i = 0; i < circleCount; i++)
        {
            circleRadii[i] = ((i + 1) / (float)circleCount) * maxRadius;
        }

        GeneratePolarGrid();
    }

    void GeneratePolarGrid()
    {
        DrawCircles();
        DrawRadialSections();
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
                points[j] = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle)); // XZ plane
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
            Vector3 start = new Vector3(innerRadius * Mathf.Cos(angle), 0, innerRadius * Mathf.Sin(angle)); // XZ plane
            Vector3 end = new Vector3(maxRadius * Mathf.Cos(angle), 0, maxRadius * Mathf.Sin(angle));

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
        lr.material = lineMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = false;
        return lr;
    }

    void Update()
    {
        DetectSectorSelection();
    }

    void DetectSectorSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Detect click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 worldPos = hit.point;
                worldPos.y = 0; // Ensure we are detecting on the XZ plane

                float distance = new Vector2(worldPos.x, worldPos.z).magnitude; // Radius from center
                float angle = Mathf.Atan2(worldPos.z, worldPos.x) * Mathf.Rad2Deg;

                if (angle < 0) angle += 360; // Normalize angle to 0-360

                int selectedCircle = -1;
                int selectedSector = (int)(angle / sectionAngle);
                
                for (int i = 0; i < circleCount; i++)
                {
                    if (distance <= circleRadii[0]) break; // first circle not selectable
                    if (distance <= circleRadii[i])
                    {
                        selectedCircle = i;
                        break;
                    }
                }

                if (selectedCircle != -1)
                {
                    Debug.Log($"Selected Sector: Circle {selectedCircle + 1}, Section {selectedSector + 1}");
                }
                else
                {
                    Debug.Log("Clicked outside the grid.");
                }
            }
        }
    }
}
