using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{

    public delegate void GameWon();
    public static event GameWon onGameWon;

    public delegate void ObjectivesDelegate();
    public static event ObjectivesDelegate onObjectivesGenerated;

    public delegate void ObjectivesCompleted();
    public static event ObjectivesCompleted onObjectivesCompleted;

    private static ObjectiveManager _instance;
    public static ObjectiveManager Instance => _instance;

    [SerializeField] private Dictionary<ItemData, (int, int)> _objectives;
    [SerializeField, Min(1), Tooltip("The minimum amount needed for objective.")] private int _lowerLimit = 1;
    [SerializeField, Tooltip("The maximum amount needed for objective.")] private int _upperLimit = 3;

    [SerializeField] private int _nrOfObjectives = 5;

    private System.Random _random;

    public int NeededAmount => _upperLimit;
    public Dictionary<ItemData, (int, int)> Objectives => _objectives;

    //Fisher-Yates shuffle
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + (int)(_random.NextDouble() * (n - i));
            T t = list[r];
            list[r] = list[i];
            list[i] = t;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        _random = new System.Random();
    }

    private void Start()
    {
        GenerateObjectives();
    }

    private void GenerateObjectives()
    {
        List<ItemData> items = ItemManager.Instance.PossibleItems;
        Shuffle(items);
        _objectives = new Dictionary<ItemData, (int, int)>();
        for (int i = 0; i < _nrOfObjectives; i++)
        {
            (int, int) tuple = (0, Random.Range(_lowerLimit, _upperLimit + 1));
            _objectives[items[i]] = tuple;
        }

        if (onObjectivesGenerated != null)
        {
            onObjectivesGenerated.Invoke();
        }
        
    }

    private void OnEnable()
    {
        ItemManager.onItemPickedUp += TrackItem;
    }

    private void OnDisable()
    {
        ItemManager.onItemPickedUp -= TrackItem;
    }

    private void TrackItem(ItemData item)
    {
        if (_objectives.ContainsKey(item))
        {
            var tuple = _objectives[item];
            tuple.Item1++;
            _objectives[item] = tuple;

        }
        CheckWin();
    }
    
    private void CheckWin()
    {
        bool isWin = true;
        foreach (var item in _objectives)
        {
            if (item.Value.Item1 < item.Value.Item2)
            {
                isWin = false;
                break;
            }
        }
        if (isWin)
        {
            if (onObjectivesCompleted != null)
            {
                onObjectivesCompleted.Invoke();
            }
        }
    }

    public void Win()
    {
        DisplayWin();
    }

    private void DisplayWin()
    {
        foreach (var item in _objectives.Values)
        {
            if (item.Item1 < item.Item2)
            {
                break;
            }
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (onGameWon != null)
        {
            onGameWon.Invoke();
        }
        
    }
}
