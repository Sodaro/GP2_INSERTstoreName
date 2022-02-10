using UnityEngine;

public class TutorialManager : MonoBehaviour
{


    [Header("LEVEL1")]
    [SerializeField] private bool _isFirstLevel = true;
    [SerializeField] private GameObject _pressTab;


    [SerializeField] private GameObject _pressPickUp;


    [SerializeField] private GameObject _pressSpace;


    [Header("Level2")]
    [SerializeField] private string _pickupSound;
    [SerializeField] private bool _isSecondLevel = false;
    [SerializeField] private GameObject _pressRMB;


    private GameObject _activePrompt;

    private bool _tabComplete = false;
    private bool _hoverComplete = false;
    private bool _pickupComplete = false;



    float randomNumber;


    private void OnEnable()
    {
        if (_isFirstLevel)
        {
            LoadingScreen.onLoadingScreenFinished += ToggleOnTab;
            MainMenu.onTabPressed += ToggleOffTab;
            PlayerInteraction.onItemHover += ToggleOnPickUp;
            PlayerInteraction.onItemInteract += ToggleOnSpace;
            ItemManager.onItemPickedUp += ToggleOffSpace;
        }
        else if (_isSecondLevel)
        {
            PlayerInteraction.onItemInteract += OnPlayerInteract;
        }
        ObjectiveManager.onGameWon += OnGameWon;
        ResumeGame.onResumeGame += OnResumeGame;
    }
    private void OnDisable()
    {
        if (_isFirstLevel)
        {
            LoadingScreen.onLoadingScreenFinished -= ToggleOnTab;
            PlayerInteraction.onItemHover -= ToggleOnPickUp;
            PlayerInteraction.onItemInteract -= ToggleOnSpace;
            MainMenu.onTabPressed -= ToggleOffTab;
            ItemManager.onItemPickedUp -= ToggleOffSpace;
        }
        else if (_isSecondLevel)
        {
            PlayerInteraction.onItemInteract -= OnPlayerInteract;
        }

        ObjectiveManager.onGameWon -= OnGameWon;
        ResumeGame.onResumeGame -= OnResumeGame;
    }


    private void OnGameWon()
    {
        if (_activePrompt != null)
            _activePrompt.SetActive(false);
    }
    private void OnResumeGame()
    {
        if (_activePrompt != null)
            _activePrompt.SetActive(true);
    }

    private void Start()
    {
        if (_isFirstLevel)
        {
            randomNumber = Random.Range(45f, 50f);
            TutorialTimer.Create(RandomEventLvL1, randomNumber, "TimerRandom");
        }
    }
    

    private void OnPlayerInteract(bool wasPickedUp)
    {
        if (wasPickedUp)
        {
            _activePrompt = _pressRMB;
            _pressRMB.SetActive(true);
            AudioManager.Instance.PlayVoiceLine(_pickupSound);
            
        }
        else
        {
            _activePrompt = null;
            _pressRMB.SetActive(false);
            PlayerInteraction.onItemInteract -= OnPlayerInteract;
        }
    }

    private void RandomEventLvL1()
    {
        AudioManager.Instance.PlayVoiceLine("Level1A_RandEvent1");
    }

    private void ToggleOnTab()
    {
        _activePrompt = _pressTab;
        _pressTab.SetActive(true);
        //Debug.Log("Toggle ON");
        AudioManager.Instance.PlayVoiceLine("Level1A_Event1");
    }

    private void ToggleOffTab()
    {

        if (_pressTab.activeInHierarchy)
        {
            _pressTab.SetActive(false);
            _tabComplete = true;
            MainMenu.onTabPressed -= ToggleOffTab;
            _activePrompt = null;
        }
        

    }

    private void ToggleOnSpace(bool _)
    {

        if (_hoverComplete)
        {
            _pickupComplete = true;
            PlayerInteraction.onItemInteract -= ToggleOnSpace;
            AudioManager.Instance.PlayVoiceLine("Tutorial_OnFirstPickUp");
            _pressPickUp.SetActive(false);
            _pressSpace.SetActive(true);
            _activePrompt = _pressSpace;
        }

    }

    private void ToggleOffSpace(ItemData _)
    {
        if (_pickupComplete)
        {
            ItemManager.onItemPickedUp -= ToggleOffSpace;
            //_spaceComplete = true;
            _pressSpace.SetActive(false);
            _activePrompt = null;
        }
    }

    private void ToggleOnPickUp(bool _)
    {
        if (_tabComplete)
        {
            _hoverComplete = true;
            PlayerInteraction.onItemHover -= ToggleOnPickUp;
            _pressPickUp.SetActive(true);
            _activePrompt = _pressPickUp;
        }

    }
}