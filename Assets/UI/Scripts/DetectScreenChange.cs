using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DetectScreenChange : MonoBehaviour
{
    private static DetectScreenChange _instance;
    private Vector2 resolution;
    public UnityEvent detectChange;
    public static DetectScreenChange Instance
    {
        get { return _instance; }
        set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else
            {
                Destroy(value.gameObject);
            }
        }
    }
    private void Awake()
    {
        Instance = this;
        resolution = new Vector2(Screen.width, Screen.height);
    }
    private void Update()
    {
        
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            detectChange.Invoke();
            resolution.x = Screen.width;
            resolution.y = Screen.height;           
        }
    }   
}
