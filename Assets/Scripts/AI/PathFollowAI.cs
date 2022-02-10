using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class PathFollowAI : MonoBehaviour
{

    [SerializeField] private AudioSource _footstepAudioSource;
    [SerializeField] private AudioSource _gaspAudioSource;
    [SerializeField] private AIVision _vision;
    [SerializeField] private float _timeBeforeStopChasing = 2f;

    [Header("Path")]
    [SerializeField] private List<Vector3> _pathPoints = new List<Vector3>();
    [SerializeField] private Color[] _colors;

    [Header("Movement Speeds")]
    [SerializeField] private float _patrollingSpeed;
    [SerializeField] private float _chasingSpeed;
    [SerializeField] private float _chasingSlowedSpeed;


    private bool _pitchSwitch = false;

    private Vector3 _targetPoint;

    private NavMeshAgent _agent;

    private int _index = 0;
    private bool _isVisitingPoint = false;

    private float _timeSinceLastCheck = 0f;
    private float _timeBetweenChecks = 0.5f;


    [SerializeField] private float _timeBetweenFootsteps = 0.5f;
    private float _soundTimer;


    private bool _isChasingPlayer = false;

    private float _timeNotSeenPlayer;


    private bool _gamePaused;
    private Animator _animator;

    private enum State { Patrolling, Chasing };

    private State _currentState;

    public List<Vector3> Path => _pathPoints;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (_pathPoints.Count == 0)
            return;

        _animator.SetBool("IsWalking", true);
        _targetPoint = _pathPoints[_index];
        _agent.destination = _targetPoint;
    }

    private void OnEnable()
    {
        ObjectiveManager.onGameWon += DisableMovement;
        ResumeGame.onResumeGame += EnableMovement;
    }

    private void OnDisable()
    {
        ObjectiveManager.onGameWon -= DisableMovement;
        ResumeGame.onResumeGame -= EnableMovement;
    }

    private void EnableMovement()
    {
        _agent.speed = _currentState == State.Patrolling ? _patrollingSpeed : _chasingSpeed;
        _gamePaused = false;
    }

    private void DisableMovement()
    {
        _agent.speed = 0f;
        _gamePaused = true;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _pathPoints.Count; i++)
        {
            int colorIndex = i % _colors.Length;
            Gizmos.color = _colors[colorIndex];
            Gizmos.DrawSphere(_pathPoints[i], 0.5f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_gamePaused)
            return;

        if (other.transform == _vision.PlayerTransform)
        {
            GameOver.Instance.TriggerGameOver();
        }
    }

    public void AddPathPoint()
    {
        Vector3 pos;
        if (_pathPoints.Count > 0)
        {
            pos = _pathPoints[_pathPoints.Count - 1];
        }
        else
        {
            pos = transform.position;
        }
        pos += Vector3.forward;
        _pathPoints.Add(pos);
    }

    public void SetPositionOfIndex(int index, Vector3 pos)
    {
        if (index < 0 || index >= _pathPoints.Count)
            return;

        _pathPoints[index] = pos;
    }



    private IEnumerator VisitPoint()
    {
        _isVisitingPoint = true;

        _animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("IsWalking", true);
        _isVisitingPoint = false;

        GetNextPoint();
    }

    private IEnumerator AggroPlayer()
    {
        _agent.ResetPath();

        yield return new WaitForSeconds(1f);
        _isChasingPlayer = true;
        _animator.SetBool("IsWalking", true);

        
    }

    private void SetActivePointAsDestination()
    {
        _targetPoint = _pathPoints[_index];
        _agent.destination = _targetPoint;
    }

    private void GetNextPoint()
    {
        if (_isChasingPlayer)
        {
            _agent.destination = _vision.PlayerTransform.position;
            return;
        }
        if (_pathPoints.Count == 0)
            return;
        
        _index++;

        _index %= _pathPoints.Count;
        SetActivePointAsDestination();
    }

    private IEnumerator SlowDebuff()
    {
        _agent.speed = _chasingSlowedSpeed;
        yield return new WaitForSeconds(2f);
        _agent.speed = _chasingSpeed;
    }

    public void HitWithObject()
    {
        if (_currentState == State.Patrolling)
        {
            ChangeState(State.Chasing);
        }
        else 
        {
            StartCoroutine(SlowDebuff());
        }
    }

    private void ChangeState(State targetState)
    {
        
        _currentState = targetState;
        if (targetState == State.Chasing)
        {
            _agent.speed = _chasingSpeed;
            _animator.SetTrigger("Surprised");
            AudioManager.Instance.AddToCombat(this);
            _gaspAudioSource.Play();
            StartCoroutine(AggroPlayer());
            
        }
        else if (targetState == State.Patrolling)
        {
            _agent.speed = _patrollingSpeed;
            _isChasingPlayer = false;
            SetActivePointAsDestination();
            AudioManager.Instance.RemoveFromCombat(this);
        }
    }

    private void UpdateDestination()
    {
        if (_currentState == State.Patrolling)
        {
            if (_timeSinceLastCheck < _timeBetweenChecks)
            {
                _timeSinceLastCheck += Time.deltaTime;
            }
            else
            {
                if (_agent.remainingDistance < 0.1f)
                {
                    StartCoroutine(VisitPoint());
                }
                _timeSinceLastCheck = 0;
            }
        }
        else
        {
            _agent.destination = _vision.PlayerTransform.position;
            _timeSinceLastCheck = 0;
        }
    }

    private void Update()
    {
        
        if (_agent.velocity.sqrMagnitude > 1f && _soundTimer <= 0)
        {
            if (!_footstepAudioSource.isPlaying)
            {
                _pitchSwitch = !_pitchSwitch;

                _footstepAudioSource.pitch = _pitchSwitch ? 0.8f : 1f;
                _footstepAudioSource.Play();
                _soundTimer = _timeBetweenFootsteps;
            }
        }
        else
            _soundTimer -= Time.deltaTime;

        if (_currentState == State.Patrolling)
        {
            if (_vision.CanSeePlayer)
            {
                ChangeState(State.Chasing);
                return;
            }

            if (_isVisitingPoint)
                return;

            UpdateDestination();
        }
        else if (_currentState == State.Chasing)
        {
            if (_isChasingPlayer == false)
                return;

            if (_vision.CanSeePlayer)
            {
                _timeNotSeenPlayer = 0;
            }
            else
            {
                _timeNotSeenPlayer += Time.deltaTime;
                if (_timeNotSeenPlayer >= _timeBeforeStopChasing)
                {
                    _timeNotSeenPlayer = 0;
                    ChangeState(State.Patrolling);
                    return;
                }
            }
            UpdateDestination();
        }
    }
}
