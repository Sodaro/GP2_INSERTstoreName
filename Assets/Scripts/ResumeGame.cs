using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeGame : MonoBehaviour
{

    public delegate void ResumeGameDelegate();
    public static event ResumeGameDelegate onResumeGame;


    public void InvokeResume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (onResumeGame != null)
        {
            onResumeGame.Invoke();
        }
    }
}
