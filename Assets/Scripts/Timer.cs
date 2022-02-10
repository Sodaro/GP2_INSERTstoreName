using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private float timerDuration = 3f * 60;

    [SerializeField]
    private bool countDown = true;

    private float timer;

    [SerializeField]
    private TextMeshProUGUI firstMinute, secondMinute, firstSecond, secondSecond, separator;

    private float flashTimer;
    private float flashDuration = 1f;

    bool alarmPlayed = false;

    private bool alarmStarted = false;

    private void OnEnable()
    {
        ObjectiveManager.onGameWon += Flash;
        MainMenu.onLevelChanged += ResetTimer;
        LoadingScreen.onLoadingScreenFinished += StartTimer;
    }
    private void OnDisable()
    {
        ObjectiveManager.onGameWon -= Flash;
        MainMenu.onLevelChanged -= ResetTimer;
        LoadingScreen.onLoadingScreenFinished -= StartTimer;
    }

    private void Start()
    {
        ResetTimer();
    }

    private void StartTimer()
    {
        alarmStarted = true;
        alarmPlayed = false;
    }


    void Update()
    {
        if (alarmPlayed == true || alarmStarted == false)
        {
            return;
        }
        if(countDown && timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
        //else if (!countDown && timer < timerDuration)
        else if (!countDown)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay(timer);
        }

        else
        {
            
            //Flash();
        }


    }

    private void ResetTimer()
    {
        alarmStarted = false;
        if (countDown)
        {
            timer = timerDuration;
        }
        else
        {
            timer = 0;
        }
        UpdateTimerDisplay(0);
        SetTextDisplay(true);

    }

    private void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt (time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        string currentTime = string.Format("{00:00}{1:00}", minutes, seconds);
        firstMinute.text = currentTime[0].ToString();
        secondMinute.text = currentTime[1].ToString();
        firstSecond.text = currentTime[2].ToString();
        secondSecond.text = currentTime[3].ToString();
    }

    private void Flash()
    {
             
        
        if (countDown && timer != 0)
        {
            //timer = 0;
            UpdateTimerDisplay(timer);
            if (!alarmPlayed)
            {
                //AudioManager.Instance.PlaySFXRandom("Day_LevelComp", "Night_LevelComp");
                // FindObjectOfType<AudioManager>().Stop("InGameMusic");
                alarmPlayed = true;
                
            }
        }

        if (!countDown && timer != timerDuration)
        {
            //timer = timerDuration;
            UpdateTimerDisplay(timer);
            if (!alarmPlayed)
            {
                //AudioManager.Instance.PlaySFXRandom("Day_LevelComp", "Night_LevelComp");
                // FindObjectOfType<AudioManager>().Stop("InGameMusic");
                alarmPlayed = true;
                
            }
        }

        if (flashTimer <= 0)
        {
            flashTimer = flashDuration;
        }
        else if(flashTimer >= flashDuration / 2)
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(false);
        }
        else
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(true);
        }
    }

    private void SetTextDisplay(bool enabled)
    {
        firstMinute.enabled = enabled;
        secondMinute.enabled = enabled;
        firstSecond.enabled = enabled;
        secondSecond.enabled = enabled;
        separator.enabled = enabled;
    }
}
