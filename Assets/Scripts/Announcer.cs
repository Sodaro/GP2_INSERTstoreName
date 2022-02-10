using System.Collections.Generic;
using UnityEngine;

public class Announcer : MonoBehaviour
{
    [SerializeField] private bool _isNightLevel;
    [SerializeField] private bool _playStartSound = true;
    [SerializeField] private bool _playListCompleteSound = true;
    
    [SerializeField] private string[] _itemShortageVO;

    [Header("Pickup Sound")]
    [SerializeField] private string _itemPickupVO;

    [Header("Day Sounds")]
    [SerializeField] private string[] _dayLoadingFinishedVO;
    [SerializeField] private string[] _dayListCompleteVO;

    [Header("Night Sounds")]
    [SerializeField] private string[] _nightLoadingFinishedVO;
    [SerializeField] private string[] _nightListCompleteVO;


    private HashSet<ItemData> _itemsWarnedAbout;


    private void Awake()
    {
        _itemsWarnedAbout = new HashSet<ItemData>();
    }

    private void OnEnable()
    {

        if (_playStartSound)
            LoadingScreen.onLoadingScreenFinished += OnLoadingFinished;

        if (_playListCompleteSound)
            ObjectiveManager.onObjectivesCompleted += OnListComplete;

        ItemManager.onItemRunningLow += OnItemRunningLow;
    }

    private void OnDisable()
    {
        if (_playStartSound)
            LoadingScreen.onLoadingScreenFinished -= OnLoadingFinished;

        if (_playListCompleteSound)
            ObjectiveManager.onObjectivesCompleted -= OnListComplete;

        ItemManager.onItemRunningLow -= OnItemRunningLow;
    }

    private void OnItemRunningLow(ItemData item)
    {
        if (!_itemsWarnedAbout.Contains(item))
        {
            if (_itemShortageVO != null || _itemShortageVO.Length > 0)
                AudioManager.Instance.PlayVoiceLineRandom(interrupt: false, _itemShortageVO);

            _itemsWarnedAbout.Add(item);
        }
        
    }

    private void OnLoadingFinished()
    {
        if (_isNightLevel)
        {
            if (_nightLoadingFinishedVO != null && _nightLoadingFinishedVO.Length > 0)
                AudioManager.Instance.PlayVoiceLineRandom(interrupt: false, _nightLoadingFinishedVO);
        }
        else
        {
            if (_dayLoadingFinishedVO != null || _dayLoadingFinishedVO.Length > 0)
                AudioManager.Instance.PlayVoiceLineRandom(interrupt: false, _dayLoadingFinishedVO);
        }
    }

    private void OnListComplete()
    {
        if (_isNightLevel)
        {
            if (_nightListCompleteVO != null || _nightListCompleteVO.Length > 0)
                AudioManager.Instance.PlayVoiceLineRandom(interrupt: false, _nightListCompleteVO);
        }
        else
        {
            if (_dayListCompleteVO != null || _dayListCompleteVO.Length > 0)
                AudioManager.Instance.PlayVoiceLineRandom(interrupt: false, _dayListCompleteVO);
        }
    }


}
