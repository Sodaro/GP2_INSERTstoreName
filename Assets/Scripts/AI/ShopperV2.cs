using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class ShopperV2 : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private bool _destroyObjects = true;

    private List<ItemData> _targetItems;
    
    private ItemData _targetItemType;
    private GameObject _targetItemGameObject;

    private NavMeshAgent _agent;
    private Animator _animator;

    private ItemSpawner _activeSpawner;

    private int _index = 0;
    private bool _isVisitingPoint = false;

    private float _timeSinceLastCheck = 0f;
    private float _timeBetweenChecks = 1f;

    private Vector3 _lastPosition;
    private Vector3 _itemHitPosition;

    private int _idleHash;
    private int _pickupHash;

    private float _pickupDelay;

    private float _pickupRange = 0.5f;
    private bool HasValidItems() { return _targetItems != null && _targetItems.Count > 0; } 
    private bool HasValidTarget() { return _targetItemGameObject != null; }
    private IEnumerator VisitPoint()
    {
        _isVisitingPoint = true;
        bool hadValidTarget = HasValidTarget();
        bool wasHeadingTowardsTarget = _agent.destination == _itemHitPosition;
        if (hadValidTarget)
        {
            _animator.SetBool(_idleHash, true);
            float f = 0f;
            float time = 1f;
            _agent.updateRotation = false;

            Quaternion startRot = transform.rotation;
            Vector3 pos1 = transform.position;

            Vector3 pos2 = _targetItemGameObject.transform.position;
            pos2.y = pos1.y;

            Vector3 lookVector = pos2 - pos1;
            if (lookVector != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(lookVector.normalized, Vector3.up);
                while (f < time)
                {
                    f += Time.deltaTime;
                    transform.rotation = Quaternion.Lerp(startRot, lookRot, f / time);
                    yield return null;
                }

            }


            _agent.updateRotation = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, _layerMask, QueryTriggerInteraction.Collide);
            foreach (Collider collider in colliders)
            {
                if (collider != null)
                {
                    if (collider.TryGetComponent(out Door door))
                    {
                        door.Open();
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }
            _animator.SetTrigger(_pickupHash);
        }
        else
        {
            _animator.SetBool(_idleHash, true);
        }
            

        _agent.ResetPath();
        float speed = _agent.speed;
        _agent.speed = 0;
        yield return new WaitForSeconds(_pickupDelay);
        _agent.speed = speed;

        _isVisitingPoint = false;
        if (hadValidTarget && wasHeadingTowardsTarget)
        {
            HandleDeleteItem();
            GetNextItem();
        }
        else
        {
            _animator.SetBool(_idleHash, false);
            if (HasValidItems())
            {
                GetNextItem();
            }
            else
            {
                GetRandomNearbyPoint();
            }
        }
    }

    private void HandleDeleteItem()
    {
        if (!_destroyObjects)
            return;

        if (_activeSpawner != null)
        {
            _activeSpawner.RemoveItem(_targetItemGameObject);
        }
        Destroy(_targetItemGameObject);
    }
    private void GetNextItem()
    {
        if (!HasValidItems())
        {
            GetRandomNearbyPoint();
            return;
        }

        _index = (_index + 1) % _targetItems.Count;

        GetPositionOfItem();
    }

    private void GetPositionOfItem()
    {
        if (!HasValidItems() || _index >= _targetItems.Count)
            return;

        _targetItemType = _targetItems[_index];

        (ItemSpawner, GameObject) tuple = ItemManager.Instance.GetRandomItemOfDataType(_targetItemType);
        _targetItemGameObject = tuple.Item2;
        _activeSpawner = tuple.Item1;

        if (!HasValidTarget())
        {
            for (int i = _targetItems.Count - 1; i >= 0; i--)
            {
                _index = i;
                _targetItemType = _targetItems[_index];
                tuple = ItemManager.Instance.GetRandomItemOfDataType(_targetItemType);
                _targetItemGameObject = tuple.Item2;
                _activeSpawner = tuple.Item1;

                if (_targetItemGameObject == null)
                    _targetItems.RemoveAt(i);
            }            
        }

        if (HasValidTarget())
        {
            if (NavMesh.SamplePosition(_targetItemGameObject.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _itemHitPosition = hit.position;
                _agent.destination = _itemHitPosition;
                return;
            }
        }

        GetRandomNearbyPoint();
    }

    private void GetRandomNearbyPoint()
    {
        _targetItemGameObject = null;
        _agent.ResetPath();
        Vector2 randomPos = Random.insideUnitCircle * 10 + Vector2.one;
        Vector3 checkPos = transform.position + new Vector3(randomPos.x, 0, randomPos.y);
        if (NavMesh.SamplePosition(checkPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            _agent.destination = hit.position;
        }
        else if (_targetItems?.Count > 0)
        {
            GetNextItem();
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _idleHash = Animator.StringToHash("IsIdle");
        _pickupHash = Animator.StringToHash("Pickup");
    }
    private void Start()
    {

        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Pickup":
                    _pickupDelay = clip.length;
                    break;
            }
        }
    }

    private void GenerateShoppingList()
    {
        _targetItems = ItemManager.Instance.GenerateAIShoppingList();
        GetPositionOfItem();
    }

    private void OnEnable()
    {
        ItemManager.onSpawnersReady += GenerateShoppingList;
        ItemManager.onItemRemovedFromSpawner += CheckItemValidity;
    }

    private void OnDisable()
    {
        ItemManager.onSpawnersReady -= GenerateShoppingList;
        ItemManager.onItemRemovedFromSpawner -= CheckItemValidity;
    }

    private void CheckItemValidity(GameObject @gameObject)
    {
        if (@gameObject == _targetItemGameObject)
        {
            _targetItemGameObject = null;
        }
    }

    private void Update()
    {
        if (_isVisitingPoint)
            return;

        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            _animator.SetBool(_idleHash, false);
        }

        if (_timeSinceLastCheck < _timeBetweenChecks)
        {
            _timeSinceLastCheck += Time.deltaTime;
        }
        else
        {
            if (_agent.remainingDistance < _pickupRange)
            {
                StartCoroutine(VisitPoint());
            }
            else
            {
                float dist = Vector3.Distance(transform.position, _lastPosition);
                if (dist < 0.25f)
                {
                    GetRandomNearbyPoint();
                }

                _lastPosition = transform.position;
            }
            _timeSinceLastCheck = 0;
        }
    }
}
