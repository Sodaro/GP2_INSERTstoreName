using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    [SerializeField] private int _threshholdForLowItemWarning = 3;
    [SerializeField] private int _timesObjectiveCanSpawn = 1;


    private static ItemManager _instance;
    public static ItemManager Instance => _instance;


    public delegate void ReadyDelegate();
    public static event ReadyDelegate onSpawnersReady;

    public delegate void ItemDelegate(ItemData item);
    public static event ItemDelegate onItemPickedUp;
    public static event ItemDelegate onItemRemovedFromBasket;
    public static event ItemDelegate onItemRunningLow;


    public delegate void GameObjectDelegate(GameObject @gameObject);
    public static event GameObjectDelegate onItemRemovedFromSpawner;

    private ItemSpawner[] _itemSpawners;

    private List<ItemData> _spawnedItems;
    private List<ItemData> _possibleItems;
    private HashSet<ItemData> _unavailableItems;
    private Dictionary<ItemData, List<ItemSpawner>> _itemSpawnersDict;

    private Dictionary<ItemData, int> _timesItemSpawned;


    public List<ItemData> SpawnedItems => _spawnedItems;
    public List<ItemData> PossibleItems => _possibleItems;
    public HashSet<ItemData> UnavailableItems => _unavailableItems;

    private System.Random _random;
    //Fisher-Yates shuffle
    private void Shuffle<T>(T[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + (int)(_random.NextDouble() * (n - i));
            T t = arr[r];
            arr[r] = arr[i];
            arr[i] = t;
        }
    }

    private void PopulateSpawnerList()
    {
        _itemSpawners = FindObjectsOfType<ItemSpawner>();
        
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
        PopulateSpawnerList();

        _timesItemSpawned = new Dictionary<ItemData, int>();
        _itemSpawnersDict = new Dictionary<ItemData, List<ItemSpawner>>();
        _spawnedItems = new List<ItemData>();
        _possibleItems = new List<ItemData>();
        _unavailableItems = new HashSet<ItemData>();

        foreach (var spawner in _itemSpawners)
        {
            foreach (var item in spawner.ItemCategory.ItemData)
            {
                if (_possibleItems.Contains(item))
                    continue;

                _possibleItems.Add(item);
            }
            spawner.onItemRemoved += CountItemStock;
        }
    }

    private void StartSpawners()
    {
        StartCoroutine(SpawnerRoutine());
    }

    private IEnumerator SpawnerRoutine()
    {
        List<ItemData> objectives = new List<ItemData>();
        foreach (var item in ObjectiveManager.Instance.Objectives)
        {
            objectives.Add(item.Key);
        }

        int spawnersPerFrame = 2;
        int spawnersUsed = 0;
        Shuffle(_itemSpawners);
        foreach (var spawner in _itemSpawners)
        {
            ItemData objectiveItem = null;

            if (spawner == null)
                continue;

            foreach (var objective in objectives)
            {
                if (spawner.CanSpawnItem(objective))
                {
                    if (!_timesItemSpawned.ContainsKey(objective))
                    {
                        _timesItemSpawned[objective] = 0;
                    }
                    spawner.SetItemToSpawn(objective);
                    _timesItemSpawned[objective]++;

                    objectiveItem = objective;
                    if (_timesItemSpawned[objective] >= _timesObjectiveCanSpawn)
                    {
                        _unavailableItems.Add(objective);
                        objectives.Remove(objective);
                    }
                    break;
                }
            }
            bool success = spawner.SpawnItems();
            if (!success)
                continue;

            if (!_itemSpawnersDict.ContainsKey(spawner.SpawnedItemData))
            {
                _itemSpawnersDict[spawner.SpawnedItemData] = new List<ItemSpawner> { spawner };
            }
            else
            {
                _itemSpawnersDict[spawner.SpawnedItemData].Add(spawner);
            }

            if (!_spawnedItems.Contains(spawner.SpawnedItemData))
            {
                _spawnedItems.Add(spawner.SpawnedItemData);
            }

            _possibleItems.Remove(spawner.SpawnedItemData);

            spawnersUsed++;
            if (spawnersUsed >= spawnersPerFrame)
            {
                yield return null;
                spawnersUsed = 0;
            }
        }
        if (onSpawnersReady != null)
        {
            onSpawnersReady.Invoke();
        }
        
    }

    private void OnEnable()
    {
        ObjectiveManager.onObjectivesGenerated += StartSpawners;
    }

    private void OnDisable()
    {
        ObjectiveManager.onObjectivesGenerated -= StartSpawners;
    }

    public List<ItemData> GenerateAIShoppingList()
    {
        List<ItemData> list = new List<ItemData>();

        if (_spawnedItems.Count < 2)
            return null;

        int size = Random.Range(2, _spawnedItems.Count);

        while (list.Count < size)
        {
            var item = _spawnedItems[Random.Range(0, size)];
            if (!list.Contains(item))
                list.Add(item);
        }
        return list;
    }

    public (ItemSpawner, GameObject) GetRandomItemOfDataType(ItemData itemData)
    {
        foreach (var key in _itemSpawnersDict.Keys)
        {
            if (key != itemData)
                continue;

            if (_itemSpawnersDict[key].Count == 0)
                continue;

            List<ItemSpawner> spawnerList = _itemSpawnersDict[key];
            ItemSpawner spawner = spawnerList[Random.Range(0, spawnerList.Count)];
            return (spawner, spawner.GetRandomItem());
        }
        return (null, null);
    }

    public void RemoveFromSpawner(GameObject @object, ItemData item)
    {
        bool removedItem = false;

        foreach (var spawner in _itemSpawnersDict[item])
        {
            if (spawner.ContainsItem(@object))
            {
                spawner.RemoveItem(@object);
                removedItem = true;
                break;
            }
        }

        if (removedItem)
        {
            //if the player removes an item from the spawner, invoke event so AI don't try to take player item
            if (onItemRemovedFromSpawner != null)
            {
                onItemRemovedFromSpawner.Invoke(@object);
            }
        }
    }

    private void CountItemStock(ItemData item)
    {
        //Loop objectives and announce if running low
        var objectives = ObjectiveManager.Instance.Objectives;
        int itemCount = 0;

        foreach (var spawner in _itemSpawnersDict[item])
        {
            itemCount += spawner.GetCount();
        }

        if (objectives.ContainsKey(item))
        {
            if (itemCount >= 0 && itemCount <= _threshholdForLowItemWarning)
            {
                AnnounceLowStock(item);
            }
        }

        if (itemCount == 0)
        {
            //enable postits on all
            foreach (var spawner in _itemSpawnersDict[item])
            {
                spawner.EnablePostits();
            }
        }

    }
    private void AnnounceLowStock(ItemData item)
    {
        if (onItemRunningLow != null)
        {
            onItemRunningLow.Invoke(item);
        }
    }

    public void RemoveFromPlayer(ItemData item, GameObject @object)
    {
        if (onItemRemovedFromBasket != null)
        {
            onItemRemovedFromBasket.Invoke(item);
        }
    }

    public void AddToPlayer(ItemData item, GameObject @object)
    {
        if (onItemPickedUp != null)
        {
            onItemPickedUp.Invoke(item);
        }
    }
}
