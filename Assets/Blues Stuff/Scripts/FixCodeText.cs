using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FixCodeText : MonoBehaviour
{
    TMP_InputField inputFeild;
    void Start()
    {
        inputFeild.onValidateInput +=
            delegate (string s, int i, char c)
            {
                return FixChar(c);
            };
    }
    void Awake()
    {
        inputFeild = GetComponent<TMP_InputField>();
    }

    public static char FixChar(char c)
    {
        if (char.IsUpper(c))
        {
            return c;
        }
        else if (char.IsLower(c))
        {
            return char.ToUpper(c);
        }
        else if (char.IsDigit(c))
        {
            return c;
        }
        else
        {
            return '\0';
        }
    }
}
