using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ScaleToScreenRatio : MonoBehaviour
{

    CanvasScaler canvasScaler;
    DetectScreenChange detectScreenChange;
    [SerializeField] float offset = 0.5f;
    private void Awake()
    {
        detectScreenChange = DetectScreenChange.Instance;
        canvasScaler = GetComponent<CanvasScaler>();
        FixRatio();
        detectScreenChange.detectChange.AddListener(FixRatio);

    }

    //adjusts ratio to match screen ratio
    void FixRatio()
    {
        float ratio = Mathf.Abs((float)Screen.width / (float)Screen.height);
        print(ratio);
        ratio -= offset;  
        ratio = Mathf.Clamp(ratio, 0, 1);
        canvasScaler.matchWidthOrHeight= ratio;
    }
}
