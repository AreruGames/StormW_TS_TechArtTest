using TMPro;
using UnityEngine;

public class ToggledAction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool toggle = false;
    [SerializeField] private string action = "";


    public void ToggleState(bool state)
    {
        toggle = state;
        print(action + ": " + toggle.ToString());

    }
}
