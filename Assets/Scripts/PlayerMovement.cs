using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    private Vector3 movement;
    private bool unableToMoveSideways;
    private bool movementDisabled;

    private bool onGround;

    [Header("Crouching")]
    [SerializeField] private float crouchMovementSpeed = 2.5f; // Movement speed when crouched
    [SerializeField] private float crouchCameraPosition; // Where the camera is positioned during crouch, if position is set to 0 it defaults to start postion / 2
    private bool isCrouching;
    private float startHeight = 2;

    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepSounds;
    int audioIndex = 0;
    private bool audioPlaying;
    [SerializeField] float walkVolume = 1;
    [SerializeField] float crouchVolume = 0.5f;
    [SerializeField] float footstepIntervalWalk = 0.5f;
    [SerializeField] float footstepIntervalCrouch = 0.5f;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private AudioSource audioSource;

    public bool GetCrouchStatus { get { return isCrouching; } }

    #region David AI Tests
    [SerializeField] private NavMeshObstacle _navMeshObstacle;
    private float _timeSpentStationary;
    private float _timeBeforeEnablingObstacle = 0.5f;
    #endregion

    private void Start()
    {
        if(walkVolume != audioSource.volume)
        {
            audioSource.volume = walkVolume;
        }            
    }

    private void Update()
    {
        if (movementDisabled) return;
        ReceiveInput();
        PlayFootsteps();
        SetMovementInfo();
    }

    private void SetMovementInfo()
    {
        if (unableToMoveSideways)
            movement = transform.forward * movement.z;
        else
        {
            movement = transform.forward * movement.z + transform.right * movement.x;
            movement.Normalize();
        }
    }

    private void PlayFootsteps()
    {
        if (movement.x == 0 && movement.z == 0 || !onGround || audioPlaying)
            return;
        else if (unableToMoveSideways && movement.z == 0)
            return;

        StartCoroutine(PlayFootstep(audioIndex));
    }

    IEnumerator PlayFootstep(int index)
    {
        if(audioIndex == 0)
            audioSource.clip = footstepSounds[0];
        else if(audioIndex == 1)
            audioSource.clip = footstepSounds[1];

        audioPlaying = true;

        if (isCrouching)
            audioSource.volume = crouchVolume;
        else
            audioSource.volume = walkVolume;

        audioSource.Play();

        if (isCrouching)
            yield return new WaitForSeconds(footstepIntervalCrouch);
        else
            yield return new WaitForSeconds(footstepIntervalWalk);
        if (index == 0)
            audioIndex = 1;
        else if (index == 1)
            audioIndex = 0;

        audioPlaying = false;
    }

    private void ReceiveInput()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        onGround = Physics.Raycast(transform.position, Vector3.down, playerCollider.height / 2 + 0.1f);

        RestartLevel();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
        }
        else if(!Input.GetKey(KeyCode.LeftControl))
        {
           CancelCrouch();
        }

    }

    private void Crouch()
    {
        cameraController.SetCameraPosition(cameraController.CameraStartPos - crouchCameraPosition, cameraController.CrouchLerpSpeed);
        playerCollider.center = new Vector3(0, -0.50f, 0);
        playerCollider.height = startHeight / 2;
        isCrouching = true;

    }

    private void CancelCrouch()
    {
        cameraController.SetCameraPosition(cameraController.CameraStartPos, cameraController.CrouchToWalkSpeed);
        playerCollider.height = startHeight;
        playerCollider.center = new Vector3(0, 0, 0);

        isCrouching = false;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleObstacleToggling();
    }

    private void HandleObstacleToggling()
    {
        //If we haven't made any movement input in a while, enable navmeshobstacle so npcs don't try to walk through us
        if (Mathf.Approximately(movement.sqrMagnitude, 0))
        {
            _timeSpentStationary += Time.fixedDeltaTime;
        }
        else
        {
            _timeSpentStationary = 0;
            _navMeshObstacle.enabled = false;
        }

        if (_timeSpentStationary > _timeBeforeEnablingObstacle)
        {
            _navMeshObstacle.enabled = true;
        }
        
    }

    private static void RestartLevel()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            MainMenu.Instance.RestartLevel();
        }
    }

    private void HandleMovement()
    {
        float velocity = -0.6f;

        if(!isCrouching)
        rb.velocity = new Vector3(movement.x, velocity, movement.z) * movementSpeed + new Vector3(0,rb.velocity.y,0);
        else if(isCrouching)
            rb.velocity = new Vector3(movement.x, velocity, movement.z) * crouchMovementSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    public void DisableSideMovement()
    {
        unableToMoveSideways = true;
    }
    public void EnableSideMovement()
    {
        unableToMoveSideways = false;
    }
}
