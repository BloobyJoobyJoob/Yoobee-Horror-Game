using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Singleton = null;

    public Color ButtonHoverColor;
    public Color ButtonNormalColor;
    public Color ButtonPressColor;
    public float ButtonPressTime;
    public float ButtonHoverTime;
    public float ButtonPressScale;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError("BNlue you ideiot");
        }
    }
}
