using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] private ItemDisplay _itemDisplayPrefab;
    [SerializeField] private Transform _itemDisplayContainer;
    //[SerializeField] private ObjectiveManager _objectiveTracker;
    

    private Dictionary<ItemData, ItemDisplay> _items;

    private void Awake()
    {
        _items = new Dictionary<ItemData, ItemDisplay>();
    }

    private void CreateUI()
    {
        foreach (var item in ObjectiveManager.Instance.Objectives)
        {
            _items[item.Key] = Instantiate(_itemDisplayPrefab, _itemDisplayContainer);

            _items[item.Key].SetCount(0);
            _items[item.Key].SetImageSprite(item.Key);
            _items[item.Key].SetNeededAmount(item.Value.Item2);
            _items[item.Key].SetName(item.Key.DisplayName);
        }
    }

    private void OnEnable()
    {
        ItemManager.onItemPickedUp += IncreaseItemScore;
        ItemManager.onItemRemovedFromBasket += DecreaseItemScore;
        ObjectiveManager.onObjectivesGenerated += CreateUI;
        MainMenu.onLevelChanged += ResetUI;
    }

    private void OnDisable()
    {
        ItemManager.onItemPickedUp -= IncreaseItemScore;
        ItemManager.onItemRemovedFromBasket -= DecreaseItemScore;
        ObjectiveManager.onObjectivesGenerated -= CreateUI;
        MainMenu.onLevelChanged -= ResetUI;
    }

    private void ResetUI()
    {
        foreach (ItemDisplay item in _items.Values)
        {
            Destroy(item.gameObject);
        }
        _items.Clear();
    }

    private void IncreaseItemScore(ItemData item)
    {
        if (!_items.ContainsKey(item))
        {
            return;
        }
        if (_items[item].Count < _items[item].Max)
        {
            AudioManager.Instance.PlaySFX("CorrectItem2");
        }
        
        _items[item].IncreaseCount(1);
    }
    private void DecreaseItemScore(ItemData item)
    {
        if (!_items.ContainsKey(item))
        {
            return;
        }
        _items[item].DecreaseCount(1);
    }
}
