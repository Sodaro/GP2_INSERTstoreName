using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    
    private void Start()
    {
        TutorialTimer.Create(TestingAction, 3f, "Timer");
        TutorialTimer.Create(TestingAction_2, 6f, "Timer_2");
        TutorialTimer.Create(TestingAction, 9f, "Timer_3");

        TutorialTimer.StopTimer("Timer");
    }

    private void TestingAction()
    {
        Debug.Log("Testing 1");
    }

    private void TestingAction_2()
    {
        Debug.Log("Testing 2");
    }

}
