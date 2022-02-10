using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialTimer
{

    private static List<TutorialTimer> activeTimerList;
    private static GameObject initGameObject;
    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FunctionTimer_InitGameObject");
            activeTimerList = new List<TutorialTimer>();
        }
    }
    public static TutorialTimer Create(Action action, float timer, string timerName = null)
    {
        InitIfNeeded();
        GameObject gameObject = new GameObject("FunctionTimer", typeof(MonoBehaviourHook));
        TutorialTimer tutorialTimer = new TutorialTimer(action, timer, timerName, gameObject);
      
        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = tutorialTimer.Update;

        activeTimerList.Add(tutorialTimer);

        return tutorialTimer;
    }

    private static void RemoveTimer(TutorialTimer tutorialTimer)
    {
        InitIfNeeded();
        activeTimerList.Remove(tutorialTimer);
    }

    // MonoBehaviour class to access on update.
    public class MonoBehaviourHook : MonoBehaviour
    {
        public Action onUpdate;
        private void Update()
        {
            if (onUpdate != null) onUpdate();
        }
    }

    private Action action;
    private float timer;
    private string timerName;
    private GameObject gameObject;
    private bool isDestroyed;
    private TutorialTimer(Action action, float timer, string timerName, GameObject gameObject)
    {
        this.action = action;
        this.timer = timer;
        this.timerName = timerName;
        this.gameObject = gameObject;
        isDestroyed = false;
    }

    public static void StopTimer(string timerName)
    {
        for (int i = 0; i < activeTimerList.Count; i++)
        {
            if (activeTimerList[i].timerName == timerName)
            {
                // Stop this timer
                activeTimerList[i].DestroySelf();
                i--;
            }
        }
    }

    public void Update()
    {
        if (!isDestroyed) 
        { 
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                    // Trigger the action
                    action();
                    DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        isDestroyed = true;
        UnityEngine.Object.Destroy(gameObject);
        RemoveTimer(this);
    }
}
