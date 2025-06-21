using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class V2_Toggle : Toggle, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Animator animator;
    [SerializeField]
    ButtonAnimation click, enter, exit;
    public float write;
    public enum ButtonAnimation
    {
        none,
        Wiggle,
        Bounce,
        Press,
        Squish,
        Spin
        
    }
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        //Set Animation Based on toggle
        if (isOn)
        {
            animator.Play("Selected", 0, 1f);
        }
        else
        {
           
            animator.Play("Disabled", 0, 1f);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (isOn)
        {
            animator.SetBool("Selected", true);
        }
        else
        {
            animator.SetBool("Disabled", true);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);        
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public void SetStateExternal(bool state)
    {
        isOn = state;
        if (state)
        {
            
            animator.SetBool("Selected", true);
        }
        else
        {
            animator.SetBool("Disabled", true);
        }
    }
}
