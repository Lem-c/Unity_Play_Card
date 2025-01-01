using UnityEngine;
using System;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;

    // Event triggered when the animation finishes
    public event Action AnimationCompleted;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not assigned or missing on AnimationManager.");
        }
    }

    /// <summary>
    /// Plays a specified animation.
    /// </summary>
    /// <param name="animationName">The name of the animation to play.</param>
    public void PlayAnimation(string animationName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        animator.Play(animationName);
        Debug.Log($"{animationName} animation started.");
    }

    /// <summary>
    /// Method to be called via Unity Animation Events at the end of an animation.
    /// </summary>
    public void OnAnimationFinished()
    {
        Debug.Log("Animation finished.");
        AnimationCompleted?.Invoke();
    }
}
