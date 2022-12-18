using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton = null;

    public UnityEvent OnPause;

    public Difficulty GameDifficulty = Difficulty.Normal;
    public State GameState = State.MainMenu;

    private bool focussed = false;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        switch (newScene.name)
        {
            case "Orphanage":
                Cursor.lockState = CursorLockMode.Locked;
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (!Application.isFocused && focussed)
        {
            focussed = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Pressed");
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Nightmare
}

public enum State
{
    MainMenu,
    MainOrphange
}