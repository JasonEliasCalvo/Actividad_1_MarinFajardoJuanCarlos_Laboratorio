using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Ajustes de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float gravity = 8;

    public KeyCode runKey = KeyCode.LeftShift;
    private bool isRunning = false;

    [SerializeField] private bool movementState;

    [Header("Ajustes de Cámara")]
    public Vector2 sensitivity = new Vector2(1f, 0.7f);

    [Header("Ajustes de GrabAndRelease")]
    [SerializeField] private Vector2 dropForce = new Vector2(50, 1);
    [SerializeField] private Transform handParent;
    [SerializeField] private Transform dropPoint;
    [HideInInspector] public bool isGrabbed = false;
    private GameObject currentGrab;
    private Rigidbody rbGrab;

    private Camera cam;
    private CharacterController chController;
    private PlayerInputActions inputActions;
    private Vector3 moveInput;
    private Vector3 lookInput;

    private Vector3 velocity;
    private float currentCameraY;

    public GameObject CurrentGrab { get => currentGrab; set => currentGrab = value; }

    // -------------------------
    // Ciclo de vida
    // -------------------------
    private void Awake()
    {
        chController = GetComponent<CharacterController>();
        instance = this;
        inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0, input.y);
        };

        inputActions.Player.Move.canceled += ctx => moveInput = Vector3.zero;

        inputActions.Player.Look.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            lookInput = new Vector3(input.x, input.y);
        };

        inputActions.Player.Look.canceled += ctx => lookInput = Vector3.zero;

        if (GameManager.instance != null)
        {
            GameManager.instance.eventGameStart += ActiveMovement;
            GameManager.instance.eventGameEnd += DeactivateMovement;
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.eventGameStart -= ActiveMovement;
            GameManager.instance.eventGameEnd -= DeactivateMovement;
        }

        inputActions.Player.Disable();
    }

    // -------------------------
    // Estados
    // -------------------------
    public void ActiveMovement() => movementState = true;
    public void DeactivateMovement() => movementState = false;

    // -------------------------
    // Loop
    // -------------------------
    private void Update()
    {
        if (UIManager.instance != null && UIManager.instance.IsPanelActive()) return;

        if (movementState)
        {
            HandleMovement();
            HandleCameraLook();
        }
    }

    private void HandleMovement()
    {
        isRunning = Input.GetKey(runKey);
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 moveDir = GetWorldMovementDirection(moveInput);
            velocity = Vector3.MoveTowards(velocity, moveDir * targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            float currentY = velocity.y;
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * Time.deltaTime);
            velocity.y = currentY;
        }

        if (!chController.isGrounded)
            velocity.y += Physics.gravity.y * gravity * Time.deltaTime;
        else
            velocity.y = -1f;

        chController.Move(velocity * Time.deltaTime);
    }

    private void HandleCameraLook()
    {
        if (lookInput.magnitude < 0.1f)
            return;

        float mouseX = lookInput.x * sensitivity.x;
        float mouseY = lookInput.y * sensitivity.y;

        // Vertical camera rotation
        currentCameraY -= mouseY;
        currentCameraY = Mathf.Clamp(currentCameraY, -80f, 50f);
        cam.transform.localRotation = Quaternion.Euler(currentCameraY, 0f, 0f).normalized;

        // Horizontal player rotation
        transform.Rotate(Vector3.up * mouseX);

    }

    // -------------------------
    // Interacciones (inputs)
    // -------------------------

    private void OnDropInput(InputAction.CallbackContext ctx)
    {

    }

    // -------------------------
    // Helpers
    // -------------------------
    private Vector3 GetWorldMovementDirection(Vector3 inputDirection)
    {
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        return (forward * inputDirection.z + right * inputDirection.x).normalized;
    }

    public void SetCam()
    {
        if (cam == null) return;
        Vector3 camEuler = cam.transform.localEulerAngles;
        if (camEuler.x > 180) camEuler.x -= 360;
        currentCameraY = camEuler.x;
    }
}
