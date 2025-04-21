using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 40f;
    public float duration = 1f;
    public Vector3 floatDirection = Vector3.up;
    public AnimationCurve fadeCurve;

    private TMP_Text text;
    private RectTransform rect;
    private float timeElapsed = 0f;
    private Color originalColor;
    void Start()
    {
        text = GetComponent<TMP_Text>();
        rect = GetComponent<RectTransform>();
        originalColor = text.color; // Capture AFTER the color is set externally
    }
    void Update()
    {
        timeElapsed += Time.deltaTime;

        // Move upward
        rect.anchoredPosition += (Vector2)(floatDirection * floatSpeed * Time.deltaTime);

        // Fade out
        float alpha = 1f - (timeElapsed / duration);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        // Destroy after finished
        if (timeElapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}