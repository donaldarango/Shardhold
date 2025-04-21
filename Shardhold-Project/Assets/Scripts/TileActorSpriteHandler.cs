using DG.Tweening;
using UnityEngine;

public class TileActorSpriteHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public bool flipSprite = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SpriteDamageAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        Color originalColor = spriteRenderer.material.color;
        mySequence.Append(spriteRenderer.material.DOColor(Color.red, 0.2f));
        mySequence.Append(spriteRenderer.material.DOColor(originalColor, 0.2f));
    }

    public void SetSpriteOrientation(Quadrant quadrant)
    {
        if (flipSprite)
        {
            spriteRenderer.flipX = true;
        }

        if (quadrant == Quadrant.NW || quadrant == Quadrant.SW) // If sprite is on left half of map
        {
            spriteRenderer.flipX = !flipSprite;
        }
    }

    public void SpawnAnimation()
    {
        // initialize starting values
        Color color = spriteRenderer.material.color;
        color.a = 0;
        spriteRenderer.material.color = color;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(spriteRenderer.material.DOFade(1, 0.5f));
    }

    public void StructureSpawnAnimation(float yOffset)
    {
        // initialize starting values
        Color color = spriteRenderer.material.color;
        color.a = 0;
        spriteRenderer.material.color = color;
        Vector3 pos = transform.position;
        pos.y += .6f;
        transform.position = pos;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(spriteRenderer.material.DOFade(1, 0.5f));
        mySequence.Append(transform.DOLocalMoveY(yOffset, .1f));
    }
}
