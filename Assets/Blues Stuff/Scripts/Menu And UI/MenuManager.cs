using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
using System.Threading.Tasks;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Singleton = null;

    [Header("Buttons")]

    public Color ButtonHoverColor;
    public Color ButtonNormalColor;
    public Color ButtonPressColor;
    public float ButtonPressTime;
    public float ButtonHoverTime;
    public float ButtonPressScale;
    public float ButtonClickDelayTime;

    public GameObject interactionStopper;

    [Header("Menus")]

    public GameObject GameMenu;
    public GameObject JoinMenu;
    public GameObject PlayMenu;

    [Header("Game Menu")]

    public TMP_InputField ClientUsername;
    public TextMeshProUGUI TeammateUsername;
    public Button StartButton;
    public Button DifficultyButton;
    public Button CodeButton;

    [Header("Random")]

    public GameObject[] DisableOnStart;

    public float CarotHeightMultiplier = 0.7f;
    public float CarotResizeDelay = 0.3f;
    private string lobbyJoinCode = "*";

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError("BNlue you ideiot");
            return;
        }

        foreach (GameObject go in DisableOnStart)
        {
            go.SetActive(false);
        }
    }
    public void Exit()
    {
        StartCoroutine(DelayButtonAction(() => {
            Application.Quit();
            Debug.Log("Quit!");
        }));
    }

    public void ToggleVisabilityDelayed(GameObject gameObject)
    {
        StartCoroutine(DelayButtonAction(() => {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }));
    }

    private IEnumerator DelayButtonAction(Action action)
    {
        interactionStopper.SetActive(true);
        yield return new WaitForSeconds(ButtonClickDelayTime);
        action();
        interactionStopper.SetActive(false);
        yield return null;
    }

    public void ThrowErrorSFX(Exception exception)
    {
        Debug.LogError(exception.Message);
    }
    
    public void PasteFromClipBoard(TMP_InputField text)
    {
        string pastedText = GUIUtility.systemCopyBuffer;

        if (pastedText.Length >= 8)
        {
            pastedText = pastedText.Substring(0, 8);
        }

        StringBuilder sb = new StringBuilder(pastedText);

        for (int i = 0; i < pastedText.Length; i++)
        {
            sb[i] = FixCodeText.FixChar(sb[i]);
        }
        text.text = sb.ToString();
    }

    public void CopyFromClipBoard()
    {
        GUIUtility.systemCopyBuffer = lobbyJoinCode;
    }

    private void ResizeCarets()
    {
        foreach (TMP_SelectionCaret item in GetComponentsInChildren<TMP_SelectionCaret>())
        {
            item.transform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y * CarotHeightMultiplier, item.transform.localScale.z);
        }
    }
    public void ResizeCaretsWithDelay()
    {
        Invoke("ResizeCarets", CarotResizeDelay + ButtonClickDelayTime);
    }

    public void OnEditUsername()
    {
        PlayerNetworkManager.ClientManager?.SetUsername(ClientUsername.text);
    }
    public async void JoinAsClient(TMP_InputField codeText)
    {
        interactionStopper.SetActive(true);
        await Task.Delay(Mathf.RoundToInt(ButtonClickDelayTime * 1000));
        await NetworkHelper.Singleton.JoinClient(ClientAttemptCallback, codeText.text);

        interactionStopper.SetActive(false);
    }
    public async void CreateAsHost()
    {
        interactionStopper.SetActive(true);
        await Task.Delay(Mathf.RoundToInt(ButtonClickDelayTime * 1000));
        await NetworkHelper.Singleton.JoinHost(ConnectAttemptCallback, true, GameManager.Singleton.GameDifficulty);
        interactionStopper.SetActive(false);
    }
    public void ClientAttemptCallback(bool started)
    {
        ConnectAttemptCallback(started, "");
    }

    public void ConnectAttemptCallback(bool started, string joinCode)
    {
        if (started)
        {
            if (joinCode == "")
            {
                CodeButton.GetComponentInChildren<TextMeshProUGUI>().text = "CoNnECtED";
            }
            else
            {
                lobbyJoinCode = joinCode;
                CodeButton.GetComponentInChildren<TextMeshProUGUI>().text = joinCode;
            }

            JoinMenu.SetActive(false);
            PlayMenu.SetActive(false);
            GameMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("Failed to connect");
        }
    }
}
public enum ConnectionFailType
{
    LobbyCreateError,
    RelayAllocationCreateError,
    RelayAllocationJoinError,
    LobbyJoinError
}