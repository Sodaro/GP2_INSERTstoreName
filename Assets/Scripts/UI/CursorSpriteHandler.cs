using UnityEngine;
using UnityEngine.UI;

public class CursorSpriteHandler : MonoBehaviour
{

    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _hoverSprite;


    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _hoverColor;

    [SerializeField] private Image _cursor;

    private void OnEnable()
    {
        PlayerInteraction.onItemHover += ChangeCursorSprite;
        PlayerInteraction.onItemInteract += ChangeCursorSprite;
    }

    private void OnDisable()
    {
        PlayerInteraction.onItemHover -= ChangeCursorSprite;
        PlayerInteraction.onItemInteract -= ChangeCursorSprite;
    }

    private void ChangeCursorSprite(bool hoverStarted)
    {
        _cursor.sprite = hoverStarted ? _hoverSprite : _defaultSprite;
        _cursor.color = hoverStarted ? _hoverColor : _defaultColor;
    }
}
