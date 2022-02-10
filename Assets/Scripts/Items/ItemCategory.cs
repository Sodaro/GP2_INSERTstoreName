using UnityEngine;
[CreateAssetMenu(fileName = "ItemCategory", menuName = "ScriptableObjects/ItemCategoryScriptableObject", order = 2)]
public class ItemCategory : ScriptableObject
{
    public string Name;
    public ItemData[] ItemData;
}
