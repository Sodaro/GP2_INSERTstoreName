using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("General Settings")]
    private float cameraStartPos; // This is the pos of the rotate object that the camera is positioned at
    [SerializeField] private float cameraSensitivity = 2;
    [SerializeField] private float cameraDistance;

    [Header("Crouch")]
    [SerializeField] private float crouchLerpSpeed = 5f;
    [SerializeField] private float crouchToWalkLerpSpeed = 5f;

    [Header("Freelook")]
    [SerializeField] private bool disableSideMovement;
    [SerializeField] private float freelookLerp = 15f;
    public bool rotateToView;
    private bool freelookOn;
    private bool lerpDone = true;
    private Quaternion savedPos;
    Quaternion newRot;

    [Header("Clamp Settings")]
    [SerializeField] private float xClampMin = -85;
    [SerializeField] private float xClampMax = 85;
    [SerializeField] private float freelookYMin = -130;
    [SerializeField] private float freelookYMax = 130;



    [Header("References")]
    public Transform objectToRotateAround;
    public Transform player;
    public PlayerMovement playerMovement;

    private float xRot;
    private float yRot;

    public float CameraStartPos { get { return cameraStartPos; } }
    public float CrouchLerpSpeed { get { return crouchLerpSpeed; } }
    public float CrouchToWalkSpeed { get { return crouchToWalkLerpSpeed; } }

    public delegate void InputEvent();
    public static event InputEvent onSpacePressed;

    private void Start()
    {
        transform.position = objectToRotateAround.position + new Vector3(0, 0, cameraDistance);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraStartPos = objectToRotateAround.localPosition.y;
    }

    private void Update()
    {
        RotateCamera();
        HandleFreeLook();

    }

    private void HandleFreeLook()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //added space event for use in tutorial
            if (onSpacePressed != null)
            {
                onSpacePressed.Invoke();
            }
            if(!freelookOn && lerpDone)
            {
                savedPos = transform.localRotation;
                if(disableSideMovement)
                playerMovement.DisableSideMovement();
            }


            freelookOn = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (freelookOn)
            {
                if(disableSideMovement)
                playerMovement.EnableSideMovement();
                lerpDone = false;

            }

            freelookOn = false;
        }

        if(rotateToView && !freelookOn && !lerpDone) // Handles rotating player to camera 
        {
            newRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.parent = null;
            player.rotation = newRot;
            transform.parent = objectToRotateAround;
            yRot = 0;
            lerpDone = true;
        }

        if(rotateToView)
            return;

        if (!lerpDone && !freelookOn)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(savedPos.x, savedPos.y, 0)), Time.deltaTime * freelookLerp);
            yRot = transform.localRotation.y;
            xRot = transform.localRotation.x;
        }
        if (transform.localRotation == Quaternion.Euler(new Vector3(savedPos.x, savedPos.y, 0)))
        {
            lerpDone = true;
            yRot = savedPos.y;
            xRot = savedPos.x;
        }
    }

    private void RotateCamera()
    {
        float yAxis = Input.GetAxis("Mouse Y") * cameraSensitivity;
        float xAxis = Input.GetAxis("Mouse X") * cameraSensitivity;

        xRot += -yAxis * cameraSensitivity;
        xRot = Mathf.Clamp(xRot, xClampMin, xClampMax);

        if (freelookOn)
        {
            yRot += xAxis;
            yRot = Mathf.Clamp(yRot, freelookYMin, freelookYMax);
            transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
            return;
        }
        if (!lerpDone)
            return;

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        player.Rotate(Vector3.up * xAxis);

    }

    public void SetCameraPosition(float yPos, float speed)
    {
        objectToRotateAround.localPosition = Vector3.Lerp(objectToRotateAround.localPosition, new Vector3(0, yPos, 0), Time.deltaTime * speed);
    }
}
