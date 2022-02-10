using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private bool _startEnabled = true;

    private void OnEnable()
    {
        ObjectiveManager.onGameWon += DisableComponents;
        ResumeGame.onResumeGame += EnableComponents;
        if (_startEnabled)
            return;

        LoadingScreen.onLoadingScreenFinished += EnableComponents;
    }

    private void OnDisable()
    {
        ObjectiveManager.onGameWon -= DisableComponents;
        ResumeGame.onResumeGame -= EnableComponents;
        if (_startEnabled)
            return;

        LoadingScreen.onLoadingScreenFinished -= EnableComponents;
    }

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
    private void Start()
    {
        if (_startEnabled == false)
        {
            _playerInteraction.enabled = false;
            _playerMovement.enabled = false;
        }
    }

    public void DisableComponents()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        _playerInteraction.enabled = false;
        _playerMovement.enabled = false;
        _cameraController.enabled = false;
    }
    public void EnableComponents()
    {
        _playerInteraction.enabled = true;
        _playerMovement.enabled = true;
        _cameraController.enabled = true;
    }
}
