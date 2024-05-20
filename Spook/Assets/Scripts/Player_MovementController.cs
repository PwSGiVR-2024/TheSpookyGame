using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Player_MovementController : MonoBehaviour
{
    [Header("Script Imports")]
    [HideInInspector] public Rigidbody Rigidbody;

    [Header("Script Exports")]
    public Transform orientation;
    private Vector3 MovementDirection;

    [Header("Movement")]
    [HideInInspector] public float MoveSpeed;

    [SerializeField] private float WalkSpeed = 5;
    [SerializeField] private float CrouchedSpeed;
    [SerializeField] private float SprintingSpeed;
    [SerializeField] private float GroundDrag;

    [Header("Crouching")]
    private readonly float CrouchingYScale = 0.75F;
    private float DefaultYScale;
    private const float CrouchForce = 5;


    [Header("KeyBinds")]
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("GroundCheck")]
    [SerializeField] private float PlayerHeight = 1.8F;
    public LayerMask IsGround;
    private bool Grounded;

    [Header("Slope Handling")]
    [SerializeField] private float MaxAngle;
    private RaycastHit SlopeHit;

    [Header("Function Checks")]
    public MovementStates State;
    private bool SprintEnabled;



    [Header("Post-Processing")]
    public PostProcessVolume PostProcessing;
    public Vignette Vignette;
    [Header("Vignette Settings")]
    [SerializeField] private float VignetteCrouched;
    [SerializeField] private float FadeDuration = 1.5F;
    [SerializeField] private float FadeTimer = 0;

    // Movement input variables
    private float HInput;
    private float VInput;

    public bool HasPlayerMoved(float MovementThreshold = 0.2F)
    {
        return Mathf.Abs(Rigidbody.velocity.magnitude) > MovementThreshold;
    }
    



    // Available Movement States
    public enum MovementStates
    {
        Standing,
        Walking,
        Sprinting,
        Crouching,
    }
    private void UpdateState()
    {
        float speed = Rigidbody.velocity.magnitude;

        if (Grounded && Input.GetKey(CrouchKey))
        {
            State = MovementStates.Crouching;
            FadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(FadeTimer / FadeDuration);
            Vignette.intensity.value = Mathf.Lerp(0, VignetteCrouched, t);
        }
        else if (speed == 0 && Grounded)
        {
            State = MovementStates.Standing;
        }
        else if (Grounded && SprintEnabled && Input.GetKey(SprintKey))
        {
            State = MovementStates.Sprinting;
        }
        else if (Grounded)
        {
            State = MovementStates.Walking;
            Vignette.intensity.value = 0;

        }
        if (State != MovementStates.Crouching)
        {
            FadeTimer = 0;
        }
    }
    private void ExecuteState()
    {
        switch (State)
        {
            case MovementStates.Standing:
                break;
            case MovementStates.Walking:
                MoveSpeed = WalkSpeed;
                break;
            case MovementStates.Sprinting:
                MoveSpeed = SprintingSpeed;
                break;
            case MovementStates.Crouching:
                MoveSpeed = CrouchedSpeed;
                break;
        }
    }

    // Load shit on awake
    private void Awake()
    {
        DefaultYScale = transform.localScale.y;
        IsGround = LayerMask.GetMask("IsGround");
        Rigidbody = GetComponent<Rigidbody>();
        _ = GetComponent<Renderer>();

        Rigidbody.freezeRotation = true;
        Rigidbody.useGravity = true;
        SprintEnabled = true;
    }
    private void Start()
    {
        if (PostProcessing && !Vignette)
        {
            PostProcessing.profile.TryGetSettings(out Vignette);
        }
        Vignette.intensity.value = 0;
    }

    private void Update()
    {
        GroundedDrag();
        PlayerInput();
        SpeedControl();
        UpdateState();
    }
    private void FixedUpdate()
    {
        PlayerMovement();
        ExecuteState();
    }


    private void GroundedDrag()
    {
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, IsGround);
        if (Grounded) Rigidbody.drag = GroundDrag;
        else Rigidbody.drag = 0;
    }

    // Player's input for the movement
    private void PlayerInput()
    {
        HInput = Input.GetAxisRaw("Horizontal");
        VInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(CrouchKey) && Grounded && !Input.GetKeyDown(SprintKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, CrouchingYScale, transform.localScale.z);
            Rigidbody.AddForce(Vector3.down * CrouchForce, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(CrouchKey) && Grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, DefaultYScale, transform.localScale.z);
        }
    }

    private void PlayerMovement()
    {
        MovementDirection = orientation.forward * VInput + orientation.right * HInput;
        Rigidbody.AddForce(10 * MoveSpeed * MovementDirection.normalized, ForceMode.Force);

        if (OnSlope())
        {
            Rigidbody.useGravity = false;
            Rigidbody.AddForce(6 * MoveSpeed * SlopeMoveDirection(), ForceMode.Force);
            if (Rigidbody.velocity.y > 0) Rigidbody.AddForce(Vector3.down * 24, ForceMode.Force);

            if (Rigidbody.velocity.y < 0) Rigidbody.AddForce(Vector3.up * 24, ForceMode.Force);
        }
        else Rigidbody.useGravity = true;
    }


    // Reduced stopping time after releasing movement
    private void SpeedControl()
    {
        if (OnSlope())
        {
            if (Rigidbody.velocity.magnitude > MoveSpeed)
            {
                Rigidbody.velocity = Rigidbody.velocity.normalized * MoveSpeed;
            }
        }
        else
        {
            Vector3 FlatVelocity = new(Rigidbody.velocity.x, 0, Rigidbody.velocity.z);

            if (FlatVelocity.magnitude > 0.1F)
            {
                Rigidbody.drag = GroundDrag;
            }
            else
            {
                Rigidbody.drag = GroundDrag * 3;
            }
            if (FlatVelocity.magnitude > MoveSpeed)
            {
                Vector3 LimitedVelocity = FlatVelocity.normalized * MoveSpeed;
                Rigidbody.velocity = new Vector3(LimitedVelocity.x, Rigidbody.velocity.y, LimitedVelocity.z);
            }
        }
    }

    // Slope calculations allowing for movement on angled surfaces
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, PlayerHeight * 0.5F + 0.3F))
        {
            float Angle = Vector3.Angle(Vector3.up, SlopeHit.normal);
            return Angle < MaxAngle && Angle != 0;
        }
        return false;
    }
    private Vector3 SlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(MovementDirection, SlopeHit.normal).normalized;
    }
}
