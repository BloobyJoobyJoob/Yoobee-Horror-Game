using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Singleton = null;

    public Color ButtonHoverColor;
    public Color ButtonNormalColor;
    public Color ButtonPressColor;
    public float ButtonPressTime;
    public float ButtonHoverTime;
    public float ButtonPressScale;

    public GameObject[] disableOnStart;

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

        foreach (GameObject go in disableOnStart)
        {
            go.SetActive(false);
        }
    }
    private void DeleteCaret()
    {
        foreach (TMP_SelectionCaret item in GetComponentsInChildren<TMP_SelectionCaret>())
        {
            item.transform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y * 0.7f, item.transform.localScale.z);
        }
    }
    public void InitPrivateLobbyMenu()
    {
        Invoke("DeleteCaret", 0.5f);
    }

    public void Exit() 
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
    
    public void PasteFromClipBoard(TMP_InputField text)
    {
        TextEditor textEditor = new();
        textEditor.multiline = true;
        textEditor.Paste();

        Debug.Log(textEditor.text);

        text.text = textEditor.text;
    }
}
