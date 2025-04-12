using System.Collections;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public TextMeshProUGUI uiText; // Or TextMeshPro if you're using world space

    public void SetDamage(int amount, bool isHealing)
    {
        uiText.text = (isHealing ? "+" : "-") + amount.ToString();
        uiText.color = isHealing ? Color.green : Color.red;

        StartCoroutine(AnimateAndDestroy());
    }

    IEnumerator AnimateAndDestroy()
    {
        float duration = 1f;
        Vector3 offset = Vector3.up * 1f;
        Vector3 start = transform.position;
        Vector3 end = start + offset;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
