using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
public class TextCountUp : MonoBehaviour
{
    public int val;
    public bool trigger = false;
    TextMeshProUGUI textMeshPro;
    [SerializeField] float duration = 0.1f, padding= 0.1f;
    // Update is called once per frame
    Coroutine counting;
    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        //StartCoroutine(Count(textMeshPro, wait));
    }
    void Update()
    {
        if (trigger && counting == null) { trigger = false; counting = StartCoroutine(Count(textMeshPro, duration, padding)); }
    }

    IEnumerator Count(TextMeshProUGUI textMeshPro, float duration, float padding)
    {
        int current = 0;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration); // Normalize time
            current = (int)Mathf.Lerp(0, val, t); // Linear interpolation
            textMeshPro.text = current.ToString();
            
            yield return new WaitForSeconds(padding);
            time += padding;
        }
        current = val;
        textMeshPro.text = current.ToString();
        counting = null;
    }
}
