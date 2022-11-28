using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Singleton = null;

    public Color ButtonHoverColor;
    public Color ButtonNormalColor;
    public Color ButtonPressColor;
    public float ButtonPressTime;
    public float ButtonHoverTime;
    public float ButtonPressScale;

    public GameObject[] DisableOnStart;

    [Header("Menus")]
    public GameObject GameMenu;
    public GameObject JoinMenu;
    public GameObject PlayMenu;

    [Header("Game Menu")]

    public TMP_InputField Player1;
    public TMP_InputField Player2;
    public Button StartButton;
    public Button DifficultyButton;
    public Button CodeButton;

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

        foreach (GameObject go in DisableOnStart)
        {
            go.SetActive(false);
        }
    }
    private void ResizeCaret()
    {
        foreach (TMP_SelectionCaret item in GetComponentsInChildren<TMP_SelectionCaret>())
        {
            item.transform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y * 0.7f, item.transform.localScale.z);
        }
    }
    public void InitPrivateLobbyMenu()
    {
        Invoke("ResizeCaret", 0.5f);
    }

    public void Exit() 
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    public void ThrowErrorSFX(ConnectionFailType failType)
    {
        Debug.LogError(failType.ToString());
    }
    
    public void PasteFromClipBoard(TMP_InputField text)
    {
        TextEditor textEditor = new();
        textEditor.multiline = true;
        textEditor.Paste();

        if (textEditor.text.Length >= 8)
        {
            textEditor.text = textEditor.text.Substring(0, 8);
        }

        StringBuilder sb = new StringBuilder(textEditor.text);

        for (int i = 0; i < textEditor.text.Length; i++)
        {
            sb[i] = FixCodeText.FixChar(sb[i]);
        }
        text.text = sb.ToString();
    }
    public void InitGameMenu()
    {
        Invoke("ResizeCaret", 0.5f);
    }

    public void OnEditPlayer1()
    {

    }
    public void OnEditPlayer2()
    {

    }
    public async void JoinAsClient(TMP_InputField codeText)
    {
        await NetworkHelper.Singleton.JoinClient(ClientCallback, codeText.text);
    }
    public async void CreateAsHost()
    {
        await NetworkHelper.Singleton.JoinHost(ConnectAttemptCallback, true);
    }

    public async void JoinPublic()
    {
        await NetworkHelper.Singleton.JoinPublic(ConnectAttemptCallback);
    }

    public void ClientCallback(bool started)
    {
        ConnectAttemptCallback(started, "");
    }

    public void ConnectAttemptCallback(bool started, string lobbyJoinCode)
    {
        if (started)
        {
            if (lobbyJoinCode == "")
            {
                CodeButton.GetComponentInChildren<TextMeshProUGUI>().text = "CoNnECtED";
            }
            else
            {
                CodeButton.GetComponentInChildren<TextMeshProUGUI>().text = lobbyJoinCode;
            }
        }
        else
        {
            CodeButton.GetComponentInChildren<TextMeshProUGUI>().text = "FaIlED";
        }
         
        JoinMenu.SetActive(false);
        GameMenu.SetActive(true);
    }
}
