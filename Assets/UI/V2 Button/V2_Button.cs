using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class V2_Button : Button, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Animator animator;
    [Header("Wait for Animation to End")]
    [SerializeField] public bool onEnd = true;
    [SerializeField] float animationSpeed = 1f;
    [SerializeField] ButtonAnimation click, enter, exit;
    public float write;
    //make sure button doesnt loose original transforms

    Vector2 pos;

    public enum ButtonAnimation
    {
        none,
        Wiggle,
        Bounce,
        Press,
        Squish,
        Spin,
        PressShadow,
        Highlight

    }
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        animator.SetFloat("speed", animationSpeed);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {

        if (click != ButtonAnimation.none)
        {
            animator.Play(Enum.GetName(click.GetType(), click), 0, 0f);
            //print(Enum.GetName(click.GetType(), click));
        }
        if (onEnd)
        {
            waitForAnimationEnd(animator, eventData);
        }
        else
        {
            base.OnPointerClick(eventData);
        }
    }

    async Task waitForAnimationEnd(Animator animator, PointerEventData eventData)
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            await Task.Delay(1);
        }
        base.OnPointerClick(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (enter != ButtonAnimation.none)
        {
            animator.SetTrigger(Enum.GetName(enter.GetType(), enter));
            //print(Enum.GetName(enter.GetType(), enter));
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Highlight"))
        {
            animator.SetTrigger("Exit");
        }
    }



    public void ResetTransform()
    {
        GetComponent<RectTransform>().localScale = Vector3.one;
        GetComponent<RectTransform>().localRotation = Quaternion.identity;
    }
}
