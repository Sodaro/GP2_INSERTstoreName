using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    //public enum ItemType { Milk, Buns, Meat, Apple, Soda, OJ, Berries, Bread, Vegetables, Cookies };
    //public static ItemDatabase Instance => _instance;
    //private static ItemDatabase _instance;

    //[SerializeField] private ItemData[] _itemDataArray;

    //[ContextMenu("Fill Data Array")]
    //public void FillItemDataArray()
    //{
    //    string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/ScriptableObjects" });
    //    int count = guids.Length;
    //    _itemDataArray = new ItemData[count];
    //    for (int n = 0; n < count; n++)
    //    {
    //        var path = AssetDatabase.GUIDToAssetPath(guids[n]);
    //        _itemDataArray[n] = AssetDatabase.LoadAssetAtPath<ItemData>(path);
    //    }
    //}

    //private void Awake()
    //{
    //    if (_instance != null && _instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        _instance = this;
    //    }
    //}
}
