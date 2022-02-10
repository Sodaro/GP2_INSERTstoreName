using UnityEngine;
using UnityEngine.Rendering;

public class BasketDimension : MonoBehaviour
{

    [SerializeField] private Transform _rotateTransform;
    [SerializeField] private Transform _holdTransform;
    [SerializeField] private Transform _cameraTransform;
    private PlayerInteraction _playerInteraction;
   

    private Camera _camera;
    private Volume _volume;

    private void Awake()
    {
        _camera = _cameraTransform.GetComponent<Camera>();
        _volume = _cameraTransform.GetComponent<Volume>();
    }

    public void Setup(PlayerInteraction playerInteraction)
    {
        //Assign transform reference to imitate rotating around player
        _playerInteraction = playerInteraction;
    }

    public Camera Camera => _camera;
    public Volume Volume => _volume;

    public Transform HoldTransform => _holdTransform;

    void Update()
    {
        //make camera copy the player camera in the basket dimension
        _rotateTransform.rotation = _playerInteraction.transform.rotation;
        _cameraTransform.rotation = Camera.main.transform.rotation;
        _cameraTransform.localRotation = Camera.main.transform.localRotation;
        _holdTransform.position = _camera.transform.position + _playerInteraction.HeldObjectCameraOffset;
    }
}
