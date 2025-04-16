using UnityEngine;

public class SpellAnimation : MonoBehaviour
{
    private Animator animator;
    private float animationLength;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("Animator or Controller not found.");
            Destroy(gameObject);
            return;
        }

        // Assumes the first animation clip is the one playing
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        if (clips.Length > 0)
        {
            animationLength = clips[0].length;
            Destroy(gameObject, animationLength);
        }
        else
        {
            Debug.LogWarning("No animation clips found.");
            Destroy(gameObject);
        }
    }
}
