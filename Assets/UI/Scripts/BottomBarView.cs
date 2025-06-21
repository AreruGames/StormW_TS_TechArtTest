using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class BottomBarView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    bool enableEdit = false;
    [SerializeField] RectTransform[] buttons;
    [SerializeField] RectTransform selector;
    [SerializeField]
    int iconsOnScreen = 5;
    [SerializeField]
    [Range(0, 100)]
    float padding = 20;
    [SerializeField]
    float buttonActiveSize = 1, animActiveSpeed = 1, animCloseSpeed = 1;

    Coroutine closeSelector;
    Coroutine openSelector;
    float buttonSize;
    float screenWidth = 1080;
    Vector2[] defaultButtonPos;
    public AnimationCurve animateActive;
    public AnimationCurve animateClose;
    public AnimationCurve animateSelector;
    public int offsetSelector = 40;
    public UnityEvent onEnter, onExit;

    public enum BottomBarButtons
    {
        Null,
        Home,
        Map,
        Shop,
        locked
    }
    private void Awake()
    {
        
    }
    void Start()
    {
        SetUpMenu();
    }

    void Update()
    {
        
        if (!Application.isPlaying && !enableEdit)
        {
            SetUpMenu();
        }
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RunClose();
        onExit.Invoke();
    }
    /// <summary>
    /// run in Editor to setup
    /// - Run when layout or screen ratio changes to allign buttons properly
    /// - layout / padding / button count 
    /// </summary>
    void SetUpMenu()
    {
        defaultButtonPos = new Vector2[buttons.Length];
        float Ratio = screenWidth / iconsOnScreen;
        //padding is percentage based
        float s_Padding = (screenWidth / 100) * padding;
        //offset off icons with padding includied
        float offset = -(screenWidth / 2);
        offset += (s_Padding / 2) + ((Ratio - (s_Padding / iconsOnScreen)) / 2);
        buttonSize = Ratio - (s_Padding / iconsOnScreen);
        int i = 0;

        //apply values to buttons
        foreach (var button in buttons)
        {
            //allows button to be interacted with
            if (button.GetComponent<HomeButton>() == null)
            {
                button.AddComponent<HomeButton>();
            }

            //sets button in correct pos and scale
            button.localPosition = new Vector3(offset, button.localPosition.y, button.localPosition.z);
            button.sizeDelta = new Vector2(buttonSize, buttonSize);

            //set icon to correct scale to match button (parent)
            RectTransform icon = button.GetChild(0).GetComponent<RectTransform>();
            if (icon != null) icon.sizeDelta = new Vector2(buttonSize, buttonSize);

            //itterate offset
            offset += Ratio - (s_Padding / iconsOnScreen);
            defaultButtonPos[i] = button.localPosition;
            
            i++;
        }

        // make selector default 0 scale
        selector.sizeDelta = new Vector2(0, 0);
    }

    public void ContentActivated(BottomBarButtons buttonID, RectTransform rt)
    {       
        print("Activated");
        float s_Padding = -((screenWidth / 100) * padding) * buttonActiveSize;
        int i = 0;
        foreach (var button in buttons)
        {
            Animator animator = button.GetComponent<Animator>();
            switch (buttonID)
            {
                case BottomBarButtons.locked:

                    if (button == rt)
                    {
                        if (animator)
                        {
                            animator.SetFloat("Speed", 1f);
                            animator.Play("Icon Lock", 0);
                        }
                    }

                    break;

                case BottomBarButtons.Map:
                    RunButtonAnimation(button, animator);
                    break;
                case BottomBarButtons.Shop:
                    RunButtonAnimation(button, animator);
                    break;
                case BottomBarButtons.Home:
                    RunButtonAnimation(button, animator);
                    break;

            }
            i++;
        }

        void RunButtonAnimation(RectTransform button, Animator animator)
        {
            if ((button == rt))
            {
                s_Padding = Mathf.Abs(s_Padding);
                StartCoroutine(AnimateButtonSelect(button, animateActive, animActiveSpeed, s_Padding));


                // ------------------- RUN SELECTOR -----------------------------------------------
                if (closeSelector != null) StopCoroutine(closeSelector); 
                openSelector = StartCoroutine(AnimateSelector(button, animateActive, animActiveSpeed, s_Padding));
                if (animator)
                {
                    animator.SetFloat("Speed", 1f);
                    animator.Play("Icon Active", 0);
                }
            }
            else
            {
                StartCoroutine(AnimateButton(button, animateActive, animActiveSpeed, s_Padding, i));
            }
        }
    }

    public void Close(BottomBarButtons buttonID)
    {
        
        switch (buttonID)
        {
            case BottomBarButtons.locked:

                break;
            case BottomBarButtons.Map:
                RunClose();
                break;
            case BottomBarButtons.Shop:
                RunClose();
                break;
            case BottomBarButtons.Home:
                RunClose();
                break;
        }
    }
    void RunClose()
    {
        print("Close");
        StopAllCoroutines();
        float s_Padding = -((screenWidth / 100) * padding) * buttonActiveSize;
        for (int i = 0; i < buttons.Length; i++)
        {
            //check if button was active and reset animation
            Animator animator = buttons[i].GetComponent<Animator>();
            if (animator)
            {
                buttons[i].GetComponent<Animator>().SetFloat("Speed", -1f);

                // ------------------- RUN SELECTOR -----------------------------------------------
                if (openSelector != null) openSelector = null;
                closeSelector = StartCoroutine(AnimateSelector(buttons[i], animateActive, animCloseSpeed, s_Padding, true));
            }
            StartCoroutine(AnimateButtonClose(buttons[i], animCloseSpeed, i));
        }
    }
    IEnumerator AnimateButton(RectTransform rt, AnimationCurve animationCurve, float speed, float offset, int index)
    {

        for (float t = 0f; t <= 1f; t += Time.deltaTime * speed)
        {
            float transform = AnimateFromCurve(rt.localPosition.x, defaultButtonPos[index].x, offset * animationCurve.Evaluate(t), t);
            rt.localPosition = new Vector3(transform, rt.localPosition.y, rt.localPosition.z);
            yield return null;
        }
    }

    IEnumerator AnimateButtonSelect(RectTransform rt, AnimationCurve animationCurve, float speed, float offset)
    {
        for (float t = 0f; t <= 1f; t += Time.deltaTime * speed)
        {
            float transform = AnimateFromCurve(rt.sizeDelta.x, buttonSize, (offset * 2) * animationCurve.Evaluate(t), t);
            rt.sizeDelta = new Vector2(transform, buttonSize);


            yield return null;
        }
    }

    IEnumerator AnimateSelector(RectTransform rt, AnimationCurve animationCurve, float speed, float offset, bool close = false)
    {
        
        for (float t = 0; t <= 1f; t += Time.deltaTime * speed)
        {
            Vector3 newPosition = new(rt.localPosition.x, rt.localPosition.y - offsetSelector, 0);
            if (close)
            {
                if (openSelector != null) yield break;
                selector.sizeDelta = new Vector2(rt.sizeDelta.x, buttonSize * animateSelector.Evaluate(1 - t));              
            } 
            else
            {
                selector.localPosition = newPosition;
                float y = Mathf.Lerp(selector.sizeDelta.y, rt.sizeDelta.y * animateSelector.Evaluate(1), t);
                selector.sizeDelta = new Vector2(rt.sizeDelta.x, y);
            }                          
            yield return null;
        }
        //failsafe
        if (close)
        {
            selector.sizeDelta = new Vector2(0, 0);
        }     
    }

    IEnumerator AnimateButtonClose(RectTransform rt, float speed, int index)
    {
        float oldTransform = rt.localPosition.x;
        float oldSize = rt.sizeDelta.x;
        for (float t = 0f; t <= 1f; t += Time.deltaTime * speed)
        {
            float newTransform = Mathf.Lerp(oldTransform, defaultButtonPos[index].x, animateClose.Evaluate(t));
            rt.localPosition = new Vector3(newTransform, rt.localPosition.y, rt.localPosition.z);
            float newSize = Mathf.Lerp(oldSize, buttonSize, animateClose.Evaluate(t));
            rt.sizeDelta = new Vector2(newSize, buttonSize);
            yield return null;
        }
        //Failsafe -  makes final transform precise
        rt.localPosition = new Vector3(defaultButtonPos[index].x, rt.localPosition.y, rt.localPosition.z);
        rt.sizeDelta = new Vector2(buttonSize, buttonSize);
    }

    /// <summary>
    /// animate home button based on curve
    /// </summary>
    /// <param name="a">lerp pos a</param>
    /// <param name="defaultTransform">stored base transform</param>
    /// <param name="animationCurve">animation curve</param>
    /// <param name="time">time to call</param>
    /// <returns>new transform</returns>
    Vector2 AnimateFromCurve(Vector2 a, Vector2 b, Vector2 animationCurve, float time)
    {
        Vector2 target = b + animationCurve;
        Vector2 newTransform = Vector2.Lerp(a, target, time);
        return newTransform;
    }
    /// <summary>
    /// animate home button based on curve
    /// </summary>
    /// <param name="a">lerp pos a</param>
    /// <param name="defaultTransform">stored base transform</param>
    /// <param name="animationCurve">animation curve</param>
    /// <param name="time">time to call</param>
    /// <returns>new transform</returns>
    float AnimateFromCurve(float a, float b, float animationCurve, float time)
    {
        float target = b + animationCurve;
        float newTransform = Mathf.Lerp(a, target, time);
        return newTransform;
    }
}
