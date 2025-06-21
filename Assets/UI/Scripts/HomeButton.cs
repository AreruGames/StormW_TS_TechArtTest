using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    public BottomBarView.BottomBarButtons ButtonID;
    

    BottomBarView BottomBarView;

    void Start()
    {
        BottomBarView = GetComponentInParent<BottomBarView>();
        if (BottomBarView == null) Debug.LogWarning("Script Not Found!");
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //toggole buttons
        BottomBarView.Close(ButtonID);
        BottomBarView.ContentActivated(ButtonID, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
