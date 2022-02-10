using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptToggler : MonoBehaviour
{
    [SerializeField] private Image[] _imagesToFade;
    [SerializeField] private Image _promptImage;
    [SerializeField] private GameObject _promptContainer;
    [SerializeField] private float _timeBetweenPrompts = 1;
    [SerializeField] private float _promptDisplayTime = 1;
    [SerializeField] private float _promptFadeTime = 1;

    private Coroutine _promptCoroutine;

    private HashSet<ItemData> _shownItemPrompts;

    private float _promptTimer;
    private Queue<IEnumerator> _promptQueue;

    private void Awake()
    {
        _promptQueue = new Queue<IEnumerator>();
        _shownItemPrompts = new HashSet<ItemData>();
    }
    private void OnEnable()
    {
        MainMenu.onLevelChanged += ResetPrompts;
        ItemManager.onItemRunningLow += ShowPrompt;
    }

    private void OnDisable()
    {
        MainMenu.onLevelChanged -= ResetPrompts;
        ItemManager.onItemRunningLow -= ShowPrompt;
    }

    private void ResetPrompts()
    {
        _promptQueue.Clear();
        foreach (var image in _imagesToFade)
            image.color = Color.white;
        _promptContainer.SetActive(false);
    }

    private void Update()
    {
        if (_promptTimer > 0)
        {
            _promptTimer -= Time.deltaTime;
        }
        else if (_promptQueue.Count > 0 && _promptCoroutine == null)
        {
            _promptCoroutine = StartCoroutine(_promptQueue.Dequeue());
        }
    }

    private void ShowPrompt(ItemData item)
    {
        if (_shownItemPrompts.Contains(item))
        {
            return;
        }

        if (_promptQueue.Count > 0)
        {
            _promptQueue.Enqueue(Prompt(item.UISprite));
        }
        else
        {
            _promptCoroutine = StartCoroutine(Prompt(item.UISprite));
        }

        _shownItemPrompts.Add(item);
    }

    private IEnumerator Prompt(Sprite sprite)
    {
        _promptImage.sprite = sprite;
        foreach (var image in _imagesToFade)
            image.color = Color.white;

        _promptContainer.SetActive(true);
        yield return new WaitForSeconds(_promptDisplayTime);

        float f = 0;
        float time = _promptFadeTime;
        while (f < time)
        {
            foreach (var image in _imagesToFade)
                image.color = Color.Lerp(Color.white, Color.clear, f / time);

            f += Time.deltaTime;
            yield return null;
        }
        _promptContainer.SetActive(false);
        _promptTimer = _timeBetweenPrompts;
        _promptCoroutine = null;
    }
}
