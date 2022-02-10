using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Pickups & Item Interactions")]
    [SerializeField, Tooltip("The range at which you can pick things up (in meters)"), Range(0,5f)]
    private float _itemPickupRange = 1.5f;
    [SerializeField, Tooltip("The layers which the pickup button will check against")] private LayerMask _pickupMask;
    [SerializeField] private float _throwForce = 100f;
    [SerializeField] private Transform _holdTransform;

    [Header("Basket")]
    [SerializeField] private BasketDimension _basketDimensionPrefab;
    private Transform _basketDimensionHoldTransform;
    private MeshFilter _dimensionFilter;
    private MeshRenderer _dimensionRenderer;

    #region Events
    public delegate void HoverEvent(bool hoveredItem);
    public static HoverEvent onItemHover;

    public delegate void InteractEvent(bool wasPickedUp);
    public static InteractEvent onItemInteract;

    #endregion

    private Rigidbody _heldBody;
    private Transform _heldTransform;
    private Item _heldItem;

    private bool _hoveringItem = false;

    private Collider _hoveredCollider;

    private float _hitDistance;
    private BasketDimension _basket;
    public Vector3 HeldObjectCameraOffset => Camera.main.transform.forward * _hitDistance;
    private void Start()
    {
        _basket = Instantiate(_basketDimensionPrefab, new Vector3(5000, 5000, 5000), Quaternion.identity).GetComponent<BasketDimension>();
        _basket.Setup(this);
        _basketDimensionHoldTransform = _basket.HoldTransform;
        _dimensionFilter = _basketDimensionHoldTransform.GetComponent<MeshFilter>();
        _dimensionRenderer = _basketDimensionHoldTransform.GetComponent<MeshRenderer>();
    }


    private void HoverSwitch(bool value)
    {
        _hoveringItem = value;
        if (onItemHover != null)
        {
            onItemHover.Invoke(value);
        }
    }

    private void Update()
    {

        if (Input.GetMouseButton(1) && _heldTransform != null)
        {
            if (onItemInteract != null)
            {
                onItemInteract.Invoke(false);
            }
            ThrowItem(drop: false);
            return;
        }


        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            if (_heldTransform != null)
            {
                if (onItemInteract != null)
                {
                    onItemInteract.Invoke(false);
                }
                ThrowItem(drop: true);
                return;
            }
            if (_hoveringItem)
            {
                if (_hoveredCollider.TryGetComponent(out Door door))
                {
                    door.ToggleOpen();
                }
                else if (_hoveredCollider.TryGetComponent(out Item item) || 
                    (_hoveredCollider.transform.parent != null && _hoveredCollider.transform.parent.TryGetComponent(out item)))
                {
                    PickupItem(_hoveredCollider.attachedRigidbody, item);
                    if (onItemInteract != null)
                    {
                        onItemInteract.Invoke(true);
                    }
                }
            }
        }

        if (_heldTransform != null)
        {
            _heldTransform.position = Camera.main.transform.position + HeldObjectCameraOffset;
            if (_heldItem.IsInsideBasketDimension)
            {
                _dimensionRenderer.enabled = true;
                _basketDimensionHoldTransform.rotation = _heldBody.transform.rotation;
                
            }
            else
            {
                _dimensionRenderer.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        Transform t = Camera.main.transform;
        if (Physics.Raycast(t.position, t.forward, out RaycastHit hit, _itemPickupRange, _pickupMask, QueryTriggerInteraction.Collide))
        {
            //layer 0 is walls / floor, which should block, the other layers are the items
            _hoveredCollider = hit.collider;
            if (_hoveredCollider.gameObject.layer == 0)
            {
                if (_hoveringItem == true && _heldTransform == null)
                {
                    HoverSwitch(false);
                }      
            }
            else if (_hoveringItem == false)
            {
                HoverSwitch(true);
            }

            if (_heldTransform != null)
            {
                _hitDistance = Mathf.Min(1f, (hit.point - Camera.main.transform.position).magnitude - 0.5f);
            }
        }
        else if (_hoveringItem == true && _heldTransform == null)
        {
            HoverSwitch(false);
            _hoveredCollider = null;
            _hitDistance = 1f;
        }
        else
        {
            _hoveredCollider = null;
            _hitDistance = 1f;
        }
    }

    private void ThrowItem(bool drop = false)
    {
        Transform t = Camera.main.transform;
        _dimensionRenderer.enabled = false;
        if (_heldItem == null)
        {
            return;
            
        }
        _heldItem.Drop();

        if (_heldItem.ItemData != null && _heldItem.IsInsideBasketDimension)
        {
            _heldItem.Teleport(_basketDimensionHoldTransform.position);
        }

        if (!drop)
        {
            _heldBody.AddForce(t.forward * _throwForce, ForceMode.Force);
            _heldItem.SetThrown(true);
        }
            

        _heldItem = null;
        _heldBody = null;
        _heldTransform = null;
    }

    private void PickupItem(Rigidbody rb, Item item)
    {
        _heldBody = rb;
        _heldItem = item;
        _heldTransform = rb.transform;

        item.PickUp(_holdTransform);
        item.transform.position = _holdTransform.position;
        if (item.ItemData == null)
        {
            _dimensionFilter.mesh = null;
            _dimensionRenderer.materials = new Material[0];
            return;
        }

        UpdatePreviewObject();

        
        
        AudioManager.Instance.PlaySFX("PickUp");
    }

    private void UpdatePreviewObject()
    {
        if (_heldTransform.TryGetComponent(out MeshFilter filter) && _heldTransform.TryGetComponent(out MeshRenderer renderer))
        {
            _dimensionFilter.mesh = filter.mesh;
            _dimensionRenderer.materials = renderer.materials;
            _basketDimensionHoldTransform.localScale = _heldTransform.transform.localScale;
        }
    }
}
