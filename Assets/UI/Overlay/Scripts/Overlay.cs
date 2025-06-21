using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Events;

public class Overlay : MonoBehaviour
{
    [Header("Popup Prefab (Required)")]
    [SerializeField] Canvas popup;
    [Header("Background Effects (Optional)")]
    [SerializeField] Camera textureRendererCamera;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] Volume volumeBlur;
    [SerializeField] Image textureRendererImage;

    Canvas[] backgroundCanvases;

    [SerializeField]
    float transitionSpeed = 1.0f, maxBlur = 3f;

    Coroutine animateBackground = null;

    private void Awake()
    {
        //Unity Event that detects screen change
        DetectScreenChange.Instance.detectChange.AddListener(GetCanvasesInScene);
        GetCanvasesInScene();
    }

    void GetCanvasesInScene()
    {
        if (UseBlur())
        {
            if (renderTexture)
            {
                renderTexture.Release();
                renderTexture.width = Screen.width;
                renderTexture.height = Screen.height;
                renderTexture.Create();
                textureRendererCamera.enabled = false;
                /*
                textureRendererCamera.enabled = false;
                await Task.Delay(1);
                textureRendererCamera.enabled = true;
            */
            }



            //setup background canvases
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("MainCanvas");
            backgroundCanvases = new Canvas[gameObjects.Length];
            for (int i = 0; i < backgroundCanvases.Length; i++)
            {
                backgroundCanvases[i] = gameObjects[i].GetComponent<Canvas>();
            }
        }
    }

    public void OpenPopup()
    {
        this.gameObject.SetActive(true);

        if (UseBlur())
        {
            //ensure camera is enabled after rendertexture updated
            //needs to refresh image resolution for texture renderer
            textureRendererCamera.enabled = true;

            if (backgroundCanvases != null)
            {
                for (int i = 0; i < backgroundCanvases.Length; i++)
                {
                    backgroundCanvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    backgroundCanvases[i].worldCamera = textureRendererCamera;
                    backgroundCanvases[i].GetComponent<GraphicRaycaster>().enabled = false;
                }
            }
            textureRendererImage.materialForRendering.SetFloat("_Blur_Amount", 0);
            volumeBlur.weight = 0;
            //setup blur effects
            if (animateBackground != null) StopCoroutine(animateBackground);
            textureRendererImage.material.SetFloat("_Blur_Amount", 0);
            volumeBlur.weight = 0;
            animateBackground = StartCoroutine(AnimatePopupBackground(textureRendererImage.material.GetFloat("_Blur_Amount"), maxBlur, volumeBlur.weight, 1, transitionSpeed));
        }
        //run popup animation
        popup.gameObject.SetActive(true);
        popup.GetComponent<Animator>().SetTrigger("Open");
        StartCoroutine(CheckPopupIdle());
    }

    public void ClosePopup()
    {
        if (UseBlur())
        {
            if (animateBackground != null) StopCoroutine(animateBackground);
            animateBackground = StartCoroutine(AnimatePopupBackground(textureRendererImage.material.GetFloat("_Blur_Amount"), 0, volumeBlur.weight, 0, transitionSpeed));
        }

        popup.GetComponent<Animator>().SetTrigger("Close");
        StartCoroutine(AnimatePopupClose());
    }

    /// <summary>
    /// Lerp render texture material and volume weight from a - b
    /// </summary>
    /// <param name="blurCurrent"> current blur amount</param>
    /// <param name="blurTarget">target blur amount</param>
    /// <param name="volumeCurrent">current volume weight</param>
    /// <param name="volumeTarget">target volume weight</param>
    /// <param name="speed">the speed of lerp</param>
    /// <returns></returns>
    IEnumerator AnimatePopupBackground(float blurCurrent, float blurTarget, float volumeCurrent, float volumeTarget, float speed)
    {
        while (true)
        {
            //Animate Background blur and volume effects
            float blur = Mathf.Lerp(blurCurrent, blurTarget, Time.deltaTime * speed);
            float volume = Mathf.Lerp(volumeBlur.weight, blurTarget, Time.deltaTime * speed);
            textureRendererImage.materialForRendering.SetFloat("_Blur_Amount", blur);
            volumeBlur.weight = volume;

            //check that blur is close to finished and break
            if (MathF.Abs(volumeBlur.weight - volumeTarget) <= 0.05f)
            {
                textureRendererImage.materialForRendering.SetFloat("_Blur_Amount", blurTarget);
                volumeBlur.weight = volumeTarget;
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Checks if animation finished before canvas can be interacted with
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPopupIdle()
    {
        while (true)
        {
            if (popup.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                popup.GetComponent<GraphicRaycaster>().enabled = true;
                break;
            }
            yield return null;
        }
    }

    IEnumerator AnimatePopupClose()
    {
        while (true)
        {
            //disable all popup things
            if (popup.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Finished"))
            {

                if (backgroundCanvases != null)
                {
                    //set background canvases
                    for (int i = 0; i < backgroundCanvases.Length; i++)
                    {
                        backgroundCanvases[i].worldCamera = Camera.main;
                        backgroundCanvases[i].GetComponent<GraphicRaycaster>().enabled = true;
                    }
                }
                popup.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    private bool UseBlur()
    {
        return textureRendererCamera != null || renderTexture != null || volumeBlur != null || textureRendererImage != null;
    }
}
