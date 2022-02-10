using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    
    private Rigidbody _rigidBody;
    private Collider _collider;

    private int _originalLayer;
    private int _playerLayer;
    private int _basketLayer;
    public ItemData ItemData => _itemData;

    private Vector3 _droppedPosition;
    private Vector3 _dimensionEnterPosition;

    private float _timeWhenDropped = 0;

    private bool _isInBasket = false;
    private bool _insideLayerTrigger = false;
    private bool _isHeld = false;

    private bool _isInSpawner = true;

    private bool _wasThrown;

    public bool WasThrown => _wasThrown;

    public void SetThrown(bool value)
    {
        _wasThrown = value;
    }

    public bool IsInsideBasketDimension => _insideLayerTrigger;
    public void PickUp(Transform holdTransform)
    {
        MakeKinematic();
        ChangeLayer(_playerLayer);
        _isHeld = true;

        transform.parent = null;
        if (_itemData == null)
            return;

        if (_isInSpawner)
        {
            ItemManager.Instance.RemoveFromSpawner(gameObject, _itemData);
            _isInSpawner = false;
        }

        if (_isInBasket)
        {
            if (ItemManager.Instance != null)
                ItemManager.Instance.RemoveFromPlayer(_itemData, gameObject);

            _timeWhenDropped = 0;
            _isInBasket = false;
        }
    }


    public void Teleport(Vector3 position)
    {
        transform.parent = null;
        transform.position = position;
    }

    private void MakeKinematic()
    {
        _rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
    }
    private void MakeNonKinematic()
    {
        _rigidBody.useGravity = true;
        _rigidBody.isKinematic = false;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void ChangeLayer(int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in transform)
            child.gameObject.layer = layer;
    }

    private void AddToBasket(Transform basketTransform)
    {
        transform.parent = basketTransform;
        _timeWhenDropped = 0;
        _isInBasket = true;
        ChangeLayer(_basketLayer);

        if (ItemManager.Instance != null)
            ItemManager.Instance.AddToPlayer(_itemData, gameObject);
    }
    private void RemoveFromBasket()
    {
        transform.parent = null;
        if (_isInBasket)
        {
            if (ItemManager.Instance != null)
                ItemManager.Instance.RemoveFromPlayer(_itemData, gameObject);
        }
        _isInBasket = false;
        ChangeLayer(_originalLayer);
    }

    public void Drop()
    {
        _timeWhenDropped = Time.time;
        _isHeld = false;
        
        if (_insideLayerTrigger == false || _itemData == null)
        {
            ChangeLayer(_originalLayer);
        }
        else
        {
            _droppedPosition = transform.position;
            ChangeLayer(_basketLayer);
        }
        MakeNonKinematic();
    }

    private void Awake()
    {
        _basketLayer = LayerMask.NameToLayer("Basket");
        _playerLayer = LayerMask.NameToLayer("Player");
        _originalLayer = gameObject.layer;
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        if (_collider == null)
            _collider = GetComponentInChildren<Collider>();
    }

    #region Unity Collision & Trigger Events
    private void OnCollisionEnter(Collision collision)
    {

        if (_wasThrown == false)
            return;

        if (collision.transform.TryGetComponent(out PathFollowAI guard))
        {
            guard.HitWithObject();
        }
        _wasThrown = false;
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("LayerChanger") && _isHeld)
        {
            _insideLayerTrigger = true;
        }
        else if (Time.time - _timeWhenDropped <= 1)
        {
            if (other.CompareTag("Basket"))
            {
                AddToBasket(other.transform);
            }
        }
        else if (other.CompareTag("BasketDimension"))
        {
            _dimensionEnterPosition = transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LayerChanger"))
        {
            _insideLayerTrigger = false;
            if (_isHeld)
            {
                ChangeLayer(_playerLayer);
            }
            else
            {
                ChangeLayer(_originalLayer);
            }
        }
        else if (other.CompareTag("BasketDimension"))
        {
            Vector3 pos = transform.position - _dimensionEnterPosition;
            Teleport(new Vector3(_droppedPosition.x + pos.x, _droppedPosition.y + pos.y, _droppedPosition.z + pos.z));
            ChangeLayer(_originalLayer);
            RemoveFromBasket();
        }
    }
    #endregion
}
