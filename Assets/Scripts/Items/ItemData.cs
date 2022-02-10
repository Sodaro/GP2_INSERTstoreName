using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemDataScriptableObject", order = 1)]
public class ItemData : ScriptableObject
{
    public string DisplayName;
    public enum ItemSize { Small, Medium, Large };
    public Sprite UISprite;
    public GameObject ItemPrefab;
    public ItemSize Size;
}