using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    private void StartTutorial()
    {
        string activeScene = SceneManager.GetActiveScene().name;


        Debug.Log(activeScene);

        if (activeScene == "MainMenu")
        {

        }
    }
    

}
