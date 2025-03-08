using DG.Tweening;
using UnityEngine;

public class TileActorSpriteHandler : MonoBehaviour
{
    private Transform cameraTransform;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraTransform = Camera.main.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTransform);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }

    public void SpriteDamageAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        Color originalColor = spriteRenderer.material.color;
        mySequence.Append(spriteRenderer.material.DOColor(Color.red, 0.2f));
        mySequence.Append(spriteRenderer.material.DOColor(originalColor, 0.2f));
    }

    public void setSpriteOrientation(Quadrant quadrant)
    {
        if (quadrant == Quadrant.NW || quadrant == Quadrant.SW) // If sprite is on left half of map
        {
            spriteRenderer.flipX = true;
        }
    }
}
