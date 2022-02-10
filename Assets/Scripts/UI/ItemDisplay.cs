using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplay : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _nameDisplay;
    [SerializeField] private TextMeshProUGUI _countDisplay;

    private int _count = 0;
    private int _max = 0;
    private string _name = "";

    public int Count => _count;
    public int Max => _max;
    public void SetNeededAmount(int amount)
    {
        _max = amount;
        //_countDisplay.text = "x"+_count.ToString();
        UpdateText();
    }

    public void SetImageSprite(ItemData item)
    {
        _image.sprite = item.UISprite;
    }
    public void SetName(string name)
    {
        _name = name;
        _nameDisplay.text = _name.ToString();
        UpdateText();
    }

    private void UpdateText()
    {
        _countDisplay.text = _count.ToString() + "/" + _max;
        _nameDisplay.text = _name;
        if (_count >= _max)
        {
            _countDisplay.text = "<s>" + _countDisplay.text + "</s>";
            _nameDisplay.text = "<s>" + _nameDisplay.text + "</s>";
        }
    }
    public void SetCount(int count)
    {
        _count = count;
        UpdateText();
    }
    
    public void IncreaseCount(int amount)
    {
        _count += amount;

        if (_count > _max)
            _count = _max;

        UpdateText();
    }
    public void DecreaseCount(int amount)
    {
        _count -= amount;
        if (_count < 0)
            _count = 0;

        UpdateText();
    }
}
