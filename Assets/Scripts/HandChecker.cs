using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HandCheckerState
{
    FullRight,
    FullLeft,
    HalfRight,
    HalfLeft
}


public class HandChecker : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Dictionary<HandCheckerState, string> animatorCommands= new Dictionary<HandCheckerState, string>() 
    { { HandCheckerState.FullRight, "FullRight" } ,{ HandCheckerState.FullLeft,"FullLeft"} 
        ,{ HandCheckerState.HalfLeft,"HalfLeft"},{ HandCheckerState.HalfRight,"HalfRight" } };

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetHandChecker(Sprite sprite, HandCheckerState state)
    {
        spriteRenderer.sprite = sprite;
        animator.SetTrigger(animatorCommands[state]);
    }
    public float ReturnAnimLength()
    {
        return animator.runtimeAnimatorController.animationClips[0].length;
    }
}
