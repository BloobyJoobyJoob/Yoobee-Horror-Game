using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton = null;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError("Blue u sucl nbals");
            return;
        }

        DontDestroyOnLoad(this);

    }
}
public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Nightmare
}
