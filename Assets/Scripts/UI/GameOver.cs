using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private static GameOver _instance;
    public static GameOver Instance => _instance;

    //public delegate void GameOverDelegate();
    //public static event GameOverDelegate onGameOver;

    [SerializeField] private float _displayTime = 2f;

    [SerializeField]
    private Image _gameOverImage;
    private bool _isGameOver;

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
    }

    public void TriggerGameOver()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;
        StartCoroutine(WaitAndRestart());
    }


    private IEnumerator WaitAndRestart()
    {
        _gameOverImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(_displayTime);
        _gameOverImage.gameObject.SetActive(false);
        _isGameOver = false;
        MainMenu.Instance.RestartLevel();
    }
}
