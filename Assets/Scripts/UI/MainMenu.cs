using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public delegate void LevelChanged();
    public static LevelChanged onLevelChanged;

    public delegate void InputEvent();
    public static InputEvent onTabPressed;

    // Options menu
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider musicSlider2;
    [SerializeField] Slider sfxSlider2;
    [SerializeField] Slider MasterSlider2;

    [SerializeField] GameObject skipBtn;
    [SerializeField] GameObject playNextBtn;

    // RESOLUTION DROPDOWN
    [SerializeField] TMP_Dropdown resolutionDropDown;
    [SerializeField] TMP_Dropdown resolutionDropDown2;
    private bool isFullScreenOn = true;
    private TMP_Text text;
    private int choosenRes;

    private List<Resolution> Resolutions = new List<Resolution>();
    private List<string> Qualitys = new List<string>();


    //QUALITY DROPDOWN
    [SerializeField] TMP_Dropdown qualityDropDown;
    [SerializeField] TMP_Dropdown qualityDropDown2;
    private TMP_Text text2;
    private int choosenQuality;


    [SerializeField] private GameObject _shoppingListUI;
    private bool isShoppingListActive = false;
    private Vector3 shoppingListStartPos;
    private Vector3 shoppingListEndPos;
    [SerializeField] private float lerpTimeUp;
    [SerializeField] private float lerpTimeDown;
    [SerializeField] private float level1ListMultiplier;
    [SerializeField] private float level2ListMultiplier;
    [SerializeField] private float level3ListMultiplier;
    [SerializeField] private float level1ListNightMultiplier;
    [SerializeField] private float level2ListNightMultiplier;
    [SerializeField] private float level3ListNightMultiplier;

    [SerializeField] private GameObject _inGamePause;
    private bool isGamePauseActive = false;

    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _defaultMenuPanel;

    [SerializeField] private GameObject _pauseGuideDay;
    [SerializeField] private GameObject _pauseGuideNight;
    [SerializeField] private GameObject _timer;

    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private GameObject _loadingScreenObject;

    [SerializeField] private GameObject _cursor;

    private void ChangedActiveScene(Scene current, Scene next)
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (skipBtn == null || playNextBtn == null)
            return;

        if (activeScene == "Level3B")
        {
            skipBtn.SetActive(false);
            playNextBtn.SetActive(false);
        }
        else
        {
            skipBtn.SetActive(true);
            playNextBtn.SetActive(true);
        }
    }

    private bool IsNightLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log($"{index}");
        return index % 2 == 0;
    }
    private bool IsNightLevel(int index)
    {
        return index % 2 == 0;
    }

    private bool _gamewon;

    const string MIXER_MUSIC = "MusicVolume";
    const string MIXER_SFX = "SFXVolume";
    const string MIXER_MASTER = "MasterVolume";

    private bool _isGamePaused;


    private bool fullscreen = true;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        _shoppingListUI.gameObject.SetActive(false);

        SetShoppingListPos(0);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        if (MasterSlider != null)
            MasterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicSlider2 != null)
            musicSlider2.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider2 != null)
            sfxSlider2.onValueChanged.AddListener(SetSFXVolume);

        if (MasterSlider2 != null)
            MasterSlider2.onValueChanged.AddListener(SetMasterVolume);
    }
    
    void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
    }

    void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
    }

    void SetMasterVolume(float value)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        ObjectiveManager.onGameWon += ShowWinPanel;
        ObjectiveManager.onObjectivesCompleted += Win;
        ResumeGame.onResumeGame += Resume;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        ObjectiveManager.onGameWon -= ShowWinPanel;
        ObjectiveManager.onObjectivesCompleted -= Win;
        ResumeGame.onResumeGame -= Resume;
    }

    private void Win()
    {
        _gamewon = true;
        
    }
    private void ShowWinPanel()
    {
        if (_gamewon)
            _winPanel.SetActive(true);
        else
            _losePanel.SetActive(true);

        _isGamePaused = true;
    }

    private void Start()
    {
        // RESOLUTION DROPDOWN
        var currentRes = Screen.currentResolution;
        var currentQuality = QualitySettings.GetQualityLevel();
        //Debug.Log(currentRes);
        //Debug.Log(currentQuality);

        Resolutions = Screen.resolutions.ToList();


        Qualitys = QualitySettings.names.ToList();

        // string[] names;
        // names = QualitySettings.names;

        foreach (var qua in Qualitys)
        {
            qualityDropDown.options.Add(new TMP_Dropdown.OptionData() { text = qua });
            qualityDropDown2.options.Add(new TMP_Dropdown.OptionData() { text = qua });
        }

        qualityDropDown.value = currentQuality;
        qualityDropDown2.value = currentQuality;


        // Print the resolutions and populate the list
        foreach (var res in Resolutions)
        {
            // Debug.Log(res.width + "x" + res.height + " : " + res.refreshRate);
            resolutionDropDown.options.Add(new TMP_Dropdown.OptionData() {text = res.width + "x" + res.height + " : " + res.refreshRate });
            resolutionDropDown2.options.Add(new TMP_Dropdown.OptionData() { text = res.width + "x" + res.height + " : " + res.refreshRate });
        }

        int currentResPos = Resolutions.IndexOf(currentRes);

        resolutionDropDown.value = currentResPos;
        resolutionDropDown2.value = currentResPos;


    }

    public void QualityChange()
    {
        if (qualityDropDown.gameObject.activeInHierarchy)
        {
            choosenQuality = qualityDropDown.value;
            QualitySettings.SetQualityLevel(choosenQuality);
        }
        
        else if (qualityDropDown2.gameObject.activeInHierarchy)
        {
            choosenQuality = qualityDropDown2.value;
            QualitySettings.SetQualityLevel(choosenQuality);
        }
    }


    public void ResolutionChange()
    {
        if (resolutionDropDown.gameObject.activeInHierarchy)
        {
            choosenRes = resolutionDropDown.value;
            int height = Resolutions[choosenRes].height;
            int width = Resolutions[choosenRes].width;
            int refresh = Resolutions[choosenRes].refreshRate;


            Screen.SetResolution(width, height, isFullScreenOn, refresh);
        }
        else if (resolutionDropDown2.gameObject.activeInHierarchy)
        {
            choosenRes = resolutionDropDown2.value;
            int height = Resolutions[choosenRes].height;
            int width = Resolutions[choosenRes].width;
            int refresh = Resolutions[choosenRes].refreshRate;

            Screen.SetResolution(width, height, isFullScreenOn, refresh);
        }
        


        
    }

    public void ToggleFullscreen()
    {
        if (Screen.fullScreen != fullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    void Update()
    {
        // Debug.Log(resolutionDropDown.value);

        if (Input.GetKeyDown(KeyCode.Tab) && SceneManager.GetActiveScene().buildIndex > 0 && _isGamePaused == false)
        {
            if (isShoppingListActive == false)
            {
                _shoppingListUI.gameObject.SetActive(true);
                isShoppingListActive = true;
                AudioManager.Instance.PlaySFX("PaperAppear");
                if (onTabPressed != null)
                {
                    onTabPressed.Invoke();
                }
            }
            else
            {
                AudioManager.Instance.PlaySFX("PaperDisappear");
                isShoppingListActive = false;
            }
                        
        }
        
        if(shoppingListStartPos != new Vector3(Screen.width / 2, -Screen.height, 0))
            SetShoppingListPos(SceneManager.GetActiveScene().buildIndex);

        if (isShoppingListActive)
            _shoppingListUI.transform.position = Vector3.Lerp(_shoppingListUI.transform.position, shoppingListStartPos + shoppingListEndPos, Time.deltaTime * lerpTimeUp);
        else if (!isShoppingListActive)
            _shoppingListUI.transform.position = Vector3.Lerp(_shoppingListUI.transform.position, shoppingListStartPos, Time.deltaTime * lerpTimeDown);

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && _isGamePaused == false)
        {
            string activeScene = SceneManager.GetActiveScene().name;
            //Debug.Log(activeScene);



            if (activeScene != "MainMenu2")
            {           
                if (isGamePauseActive == false)
                {
                    PlayerController.Instance.DisableComponents();
                    _inGamePause.gameObject.SetActive(true);
                    isGamePauseActive = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    Time.timeScale = 0;
                    if (IsNightLevel())
                    {
                        _pauseGuideNight.gameObject.SetActive(true);
                        _pauseGuideDay.gameObject.SetActive(false);
                    }
                    else
                    {
                        _pauseGuideDay.gameObject.SetActive(true);
                        _pauseGuideNight.gameObject.SetActive(false);
                    }
                }
                else
                {
                    Resume();
                }

            }


            }

    }

    

    private void ResetMenu()
    {
        foreach (Transform child in _mainMenuPanel.transform)
        {
            child.gameObject.SetActive(false);
        }
        _defaultMenuPanel.SetActive(true);
    }

    public void Resume()
    {
        _isGamePaused = false;
        PlayerController.Instance.EnableComponents();
        _inGamePause.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        isGamePauseActive = false;
    }
     


    // public void SetNewVolumeMusic(float newMusicValue)
    // {
    //     AudioManager.Instance.SetVolumeMusic(newMusicValue);
    // 
    // }

    //MAIN MENU

    private void ResetLevelObjects(bool isMainMenu)
    {
        _shoppingListUI.SetActive(false);
        _gamewon = false;
        _isGamePaused = false;
        isShoppingListActive = false;
        Time.timeScale = 1;
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
        if (onLevelChanged != null)
        {
            onLevelChanged.Invoke();
        }
        
        _loadingScreenObject.SetActive(!isMainMenu);
        
        if (isMainMenu)
        {
            ResetMenu();
            _mainMenuPanel.SetActive(true);
        }
    }

    public void PlayLEVEL1()
    {
        _cursor.SetActive(true);
        _loadingScreen.Setup(IsNightLevel(1));
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene("Level1");
        SetShoppingListPos(1);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayLEVEL2()
    {
        _cursor.SetActive(true);
        _loadingScreen.Setup(IsNightLevel(2));
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene("Level1B");
        SetShoppingListPos(2);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayLEVEL3()
    {
        _cursor.SetActive(true);
        _loadingScreen.Setup(IsNightLevel(3));
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene("Level2");
        SetShoppingListPos(3);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayLEVEL4()
    {
        _cursor.SetActive(true);
        _loadingScreen.Setup(IsNightLevel(4));
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene("Level2B");
        SetShoppingListPos(4);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayLEVEL5()
    {
        _cursor.SetActive(true);
        _loadingScreen.Setup(IsNightLevel(5));
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene("Level3");
        SetShoppingListPos(5);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayLEVEL6()
    {
        _cursor.SetActive(true);
        _loadingScreen.Setup(IsNightLevel(6));
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene("Level3B");
        SetShoppingListPos(6);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void QuitGame()
    {
        //Debug.Log("Quit game!");
        Application.Quit();
    }

    

    // IN GAME MENU

    public void PlayNextLevel()
    {
        
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene >= sceneCount)
            nextScene = 0;

        _cursor.SetActive(nextScene != 0);
        //nextScene %= sceneCount;
        _loadingScreen.Setup(!IsNightLevel());
        ResetLevelObjects(isMainMenu: nextScene == 0);
        SceneManager.LoadScene(nextScene);
        SetShoppingListPos(nextScene);
    }

    public void BackToMainMenu()
    {
        ResetLevelObjects(isMainMenu: true);
        SceneManager.LoadScene("MainMenu2");
        _timer.SetActive(false);
        
    }

    public void RestartLevel()
    {
        _loadingScreen.Setup(IsNightLevel());
        ResetLevelObjects(isMainMenu: false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetShoppingListPos(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetShoppingListPos(int index)
    {
        shoppingListStartPos = new Vector3(Screen.width / 2, -Screen.height, 0);
        if (index == 1)
        {
            shoppingListEndPos = new Vector3(0, Screen.height * level1ListMultiplier,0);
            return;
        }
        if(index == 2)
        {
            shoppingListEndPos = new Vector3(0, Screen.height * level1ListNightMultiplier, 0); ;
            return;
        }
        if (index == 3)
        {
            shoppingListEndPos = new Vector3(0, Screen.height * level2ListMultiplier, 0); ;
            return;
        }
        if (index == 4)
        {
            shoppingListEndPos = new Vector3(0, Screen.height * level2ListNightMultiplier, 0); ;
            return;
        }
        if (index == 5)
        {
            shoppingListEndPos = new Vector3(0, Screen.height * level3ListMultiplier, 0);
            return;
        }
        if (index == 6)
        {
            shoppingListEndPos = new Vector3(0, Screen.height * level3ListNightMultiplier, 0);
            return;
        }
        shoppingListEndPos = new Vector3(0, Screen.height * level1ListMultiplier, 0);

    }
}
