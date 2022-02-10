using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _loadingScreen;
    [SerializeField] private Sprite _dayScreen;
    [SerializeField] private Sprite _nightScreen;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private float _waitTime = 1f;

    public delegate void LoadingScreenDelegate();
    public static event LoadingScreenDelegate onLoadingScreenFinished;

    private void OnEnable()
    {
        ItemManager.onSpawnersReady += FadeOutLoadingScreen;
        MainMenu.onLevelChanged += ResetLoadingScreen;
    }
    private void OnDisable()
    {
        ItemManager.onSpawnersReady -= FadeOutLoadingScreen;
        MainMenu.onLevelChanged -= ResetLoadingScreen;
    }

    private void ResetLoadingScreen()
    {
        _loadingScreen.color = Color.white;
    }

    private void FadeOutLoadingScreen()
    {
        StartCoroutine(WaitAndFadeOut());
    }

    public void Setup(bool isNightLevel)
    {
        _loadingScreen.sprite = isNightLevel ? _nightScreen : _dayScreen;
    }

    private IEnumerator WaitAndFadeOut()
    {
        float f = 0;
        float time = _fadeTime;

        yield return new WaitForSeconds(_waitTime);

        Color clear = new Color(1, 1, 1, 0);

        while (f < time)
        {
            _loadingScreen.color = Color.Lerp(Color.white, clear, f / time);
            f += Time.deltaTime;
            yield return null;
        }

        _loadingScreen.color = clear;

        if (onLoadingScreenFinished != null)
        {
            onLoadingScreenFinished.Invoke();
        }
        
    }
}
