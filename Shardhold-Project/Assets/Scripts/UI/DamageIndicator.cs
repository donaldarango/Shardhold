using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float duration = 1f;
    public Vector3 floatDirection = Vector3.up;

    private TextMeshProUGUI text;
    private Color originalColor;
    private float timer = 0f;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    public void SetDamage(int damage)
    {
        text.text = damage.ToString();
    }

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        // Fade out
        float alpha = Mathf.Lerp(originalColor.a, 0, timer / duration);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
