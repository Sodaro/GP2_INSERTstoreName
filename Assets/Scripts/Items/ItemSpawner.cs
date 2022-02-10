using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _postItNotes;
    private List<SpriteRenderer> _postItSpriteRenderers;


    public delegate void ItemDelegate(ItemData item);
    public event ItemDelegate onItemRemoved;

    [Header("Spawn Positions")]
    [SerializeField] private Transform[] _spawnPositionsSmall;
    [SerializeField] private Transform[] _spawnPositionsMid;
    [SerializeField] private Transform[] _spawnPositionsLarge;
    

    [Header("Spawn Settings")]
    [SerializeField] private ItemCategory _spawnCategory;

    [SerializeField, Tooltip("Controls whether the spawned prefabs are scaled to the placed position transforms.")]
    private bool _scalePrefabs;

    [SerializeField, Tooltip("Controls whether the spawned prefabs are rotated to the placed position transforms.")]
    private bool _rotatePrefabs;

    private Transform[] _spawnPositions;
    private List<GameObject> _items;

    private ItemData _itemToSpawn;

    public ItemCategory ItemCategory => _spawnCategory;

    public ItemData SpawnedItemData => _itemToSpawn;
    private void Awake()
    {
        _items = new List<GameObject>();
        _postItSpriteRenderers = new List<SpriteRenderer>();
        foreach (var obj in _postItNotes)
        {
            _postItSpriteRenderers.Add(obj.GetComponentInChildren<SpriteRenderer>());
        }
        DisablePostits();


    }

    public int GetCount() => _items.Count;

    public void SetItemToSpawn(ItemData itemToSpawn)
    {
        _itemToSpawn = itemToSpawn;
    }

    public bool CanSpawnItem(ItemData item)
    {
        foreach (var data in _spawnCategory.ItemData)
        {
            if (data == item)
            {
                return true;
            }
        }
        return false;
    }

    private void DisablePostits()
    {
        if (_postItNotes != null && _postItNotes.Length != 0)
        {
            foreach (var note in _postItNotes)
            {
                note.SetActive(false);
            }
        }
    }

    public void EnablePostits()
    {
        if (_postItNotes != null && _postItNotes.Length != 0)
        {
            foreach (var note in _postItNotes)
            {
                note.SetActive(true);
            }
        }
    }
    private void DestroyAllSpawns()
    {
        foreach (var t in _spawnPositionsSmall)
            Destroy(t.gameObject);
        foreach (var t in _spawnPositionsMid)
            Destroy(t.gameObject);
        foreach (var t in _spawnPositionsLarge)
            Destroy(t.gameObject);

        _spawnPositions = null;
    }
    private void DestroyOtherSpawns(ItemData.ItemSize usedSize)
    {
        switch (usedSize)
        {
            case ItemData.ItemSize.Small:
                foreach (var t in _spawnPositionsMid)
                    Destroy(t.gameObject);
                foreach (var t in _spawnPositionsLarge)
                    Destroy(t.gameObject);
                break;
            case ItemData.ItemSize.Medium:
                foreach (var t in _spawnPositionsSmall)
                    Destroy(t.gameObject);
                foreach (var t in _spawnPositionsLarge)
                    Destroy(t.gameObject);
                break;
            case ItemData.ItemSize.Large:
                foreach (var t in _spawnPositionsMid)
                    Destroy(t.gameObject);
                foreach (var t in _spawnPositionsSmall)
                    Destroy(t.gameObject);
                break;
        }
    }

    public bool SpawnItems()
    {
        ItemData[] itemDatas = _spawnCategory.ItemData;

        //make sure all items spawn once, then any other item at least once
        if (_itemToSpawn == null)
        {
            for (int i = 0; i < itemDatas.Length; i++)
            {
                var item = itemDatas[i];
                if (ItemManager.Instance.UnavailableItems.Contains(item))
                    continue;

                if (!ItemManager.Instance.PossibleItems.Contains(item))
                    continue;

                _itemToSpawn = item;
            }
        }
        //all items have spawned once
        if (_itemToSpawn == null)
        {
            int i = 0;
            int maxIterations = 500;
            ItemData item = null;

            while (i < maxIterations && (item == null || ItemManager.Instance.UnavailableItems.Contains(item)))
            {
                item = itemDatas[Random.Range(0, itemDatas.Length)];
                i++;
            }
            if (i < maxIterations)
            {
                _itemToSpawn = item;
            }
            
        }

        if (_postItSpriteRenderers != null && _itemToSpawn != null && _postItSpriteRenderers.Count != 0)
        {
            foreach (var renderer in _postItSpriteRenderers)
            {
                renderer.sprite = _itemToSpawn?.UISprite;
            }
        }

        GameObject objectToSpawn = _itemToSpawn?.ItemPrefab;
        ItemData.ItemSize? size = _itemToSpawn?.Size;
        if (size != null)
        {
            switch (size)
            {
                case ItemData.ItemSize.Small:
                    _spawnPositions = _spawnPositionsSmall;
                    DestroyOtherSpawns((ItemData.ItemSize)size);
                    break;
                case ItemData.ItemSize.Medium:
                    _spawnPositions = _spawnPositionsMid;
                    DestroyOtherSpawns((ItemData.ItemSize)size);
                    break;
                case ItemData.ItemSize.Large:
                    _spawnPositions = _spawnPositionsLarge;
                    DestroyOtherSpawns((ItemData.ItemSize)size);
                    break;
            }
        }
        else
        {
            DestroyAllSpawns();
        }
  
        if (_spawnPositions == null)
            return false;

        foreach (var t in _spawnPositions)
        {
            if (objectToSpawn != null)
            {
                Vector3 pos = t.position;
                Vector3 scale = t.localScale;
                Quaternion rotation = t.rotation;

                GameObject instance = Instantiate(objectToSpawn);
                instance.transform.parent = t.parent;
                instance.transform.position = pos;

                if (_scalePrefabs)
                {
                    instance.transform.localScale = scale;
                }
                if (_rotatePrefabs)
                {
                    instance.transform.rotation = rotation;
                }

                _items.Add(instance);
            }

            Destroy(t.gameObject);
        }
        return _itemToSpawn != null;
    }

    public bool ContainsItem(GameObject @object)
    {
        return _items.Contains(@object);
    }
    public void RemoveItem(GameObject @object)
    {
        _items.Remove(@object);

        if (onItemRemoved != null)
        {
            onItemRemoved.Invoke(_itemToSpawn);
        }
    }

    public GameObject GetRandomItem()
    {
        if (_items.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, _items.Count);

        GameObject obj = _items[index];
        return obj;
    }
}
